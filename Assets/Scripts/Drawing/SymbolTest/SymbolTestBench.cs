using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SymbolTestBench : MonoBehaviour
{
    [SerializeField] private string drawer;
    [SerializeField] private LetterDrawing settings;
    [SerializeField] private DrawingRigReferences rigReferences;
    [SerializeField] private DrawingConfig drawingConfig;

    [SerializeField] private RectTransform drawZone;
    [SerializeField] private RawImage drawingDisplay;
    [SerializeField] private RawImage modelInput;
    [SerializeField] private TMP_Text guide;

    [SerializeField] private SymbolCell cellPrefab;
    [SerializeField] private Transform cellHolder;
    [SerializeField] private PredictionRow[] rows;

    [SerializeField] private TMP_Text viewTitle;
    [SerializeField] private TMP_Text targetGlyph;
    [SerializeField] private TMP_Text targetLabel;
    [SerializeField] private TMP_Text targetStats;
    [SerializeField] private TMP_Text totals;
    [SerializeField] private TMP_Text verdict;

    private readonly PlayerDrawingRig rig = new PlayerDrawingRig();
    private readonly PointCloudRecognizer templates = new PointCloudRecognizer();
    private readonly List<SymbolTemplates.Entry> built = new List<SymbolTemplates.Entry>();
    private readonly List<Vector2> samples = new List<Vector2>();

    private SymbolRecognizer recognizer;
    private DrawingStroke stroke;
    private LineRenderer visibleLine;
    private Camera displayCamera;
    private RenderTexture displayTexture;
    private SymbolCell[] cells;
    private SymbolCell target;
    private SymbolEngine view = SymbolEngine.Combined;
    private SymbolPrediction lastPrediction;
    private TemplateMatch lastMatch;
    private CombinedMatch lastCombined;
    private bool hasResult;
    private bool isDrawing;
    private bool isReplaying;

    private void Start()
    {
        SpawnRig();
        SetupDisplay();
        SpawnCells();

        recognizer = new SymbolRecognizer(settings.model, rig.Cameras.MLCamera);
        modelInput.texture = recognizer.Input;

        Select(0);
        SetView(view);
        Replay().Forget();
    }

    private void OnDestroy()
    {
        recognizer.Dispose();
        displayTexture.Release();
    }

    private void Update()
    {
        if (isReplaying)
            return;

        HandleKeys();
        HandleMouse();
    }

    private void SpawnRig()
    {
        rig.Spawn(rigReferences, drawingConfig);
        rig.Primary.enabled = false;
        rig.Cameras.MLCamera.enabled = false;

        visibleLine = rig.ModeLines[DrawingMode.Predicting];
        visibleLine.enabled = true;
        visibleLine.startWidth = settings.drawStrokeWidth;
        visibleLine.endWidth = settings.drawStrokeWidth;

        stroke = new DrawingStroke(rig.Primary);
        stroke.UseVisibleLine(visibleLine);
    }

    private void SetupDisplay()
    {
        displayCamera = rig.Cameras.DisplayCamera;
        displayCamera.enabled = true;

        Rect zone = drawZone.rect;
        float aspect = zone.width / zone.height;
        int height = Mathf.RoundToInt(zone.height * drawZone.GetComponentInParent<Canvas>().scaleFactor);

        displayTexture = new RenderTexture(Mathf.RoundToInt(height * aspect), height, 24, RenderTextureFormat.ARGB32);
        displayTexture.Create();

        displayCamera.aspect = aspect;
        displayCamera.targetTexture = displayTexture;
        drawingDisplay.texture = displayTexture;
    }

    private void SpawnCells()
    {
        cells = SymbolRecognizer.Active.Select(SpawnCell).ToArray();
    }

    private SymbolCell SpawnCell(string label)
    {
        SymbolCell cell = Instantiate(cellPrefab, cellHolder);
        cell.name = label;
        cell.Init(Array.IndexOf(SymbolRecognizer.Labels, label), clicked => Select(Array.IndexOf(cells, clicked)));

        return cell;
    }

    private void HandleKeys()
    {
        Keyboard keyboard = Keyboard.current;
        int position = Array.IndexOf(cells, target);

        if (keyboard.spaceKey.wasPressedThisFrame)
            Select(RandomOtherPosition(position));

        if (keyboard.rightArrowKey.wasPressedThisFrame)
            Select((position + 1) % cells.Length);

        if (keyboard.leftArrowKey.wasPressedThisFrame)
            Select((position + cells.Length - 1) % cells.Length);

        if (keyboard.tabKey.wasPressedThisFrame)
            SetView(Next(view));

        if (keyboard.gKey.wasPressedThisFrame)
            guide.gameObject.SetActive(!guide.gameObject.activeSelf);

        if (keyboard.rKey.wasPressedThisFrame)
            ResetStats();

        if (keyboard.pKey.wasPressedThisFrame)
            Replay().Forget();

        if (keyboard.eKey.wasPressedThisFrame)
            SaveTemplates();
    }

    private void HandleMouse()
    {
        Mouse mouse = Mouse.current;
        Vector2 position = mouse.position.ReadValue();
        bool pressed = mouse.leftButton.isPressed || mouse.rightButton.isPressed;
        bool began = mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame;

        if (began && RectTransformUtility.RectangleContainsScreenPoint(drawZone, position))
            BeginStroke();
        else if (isDrawing && pressed)
            AddSample(ScreenToWorld(position));
        else if (isDrawing)
            EndStroke();
    }

    private void BeginStroke()
    {
        isDrawing = true;
        samples.Clear();
        stroke.Begin(settings.minPointWorldDistance, float.PositiveInfinity);
    }

    private void AddSample(Vector3 point)
    {
        samples.Add(point);
        stroke.AddPoint(point);
    }

    private void EndStroke()
    {
        isDrawing = false;

        if (rig.Primary.positionCount < 2)
            return;

        SymbolPrediction prediction = recognizer.Predict(rig.Primary, visibleLine);
        TemplateMatch match = templates.Recognize(PointCloudRecognizer.Points(rig.Primary));
        CombinedMatch combined = new CombinedMatch(prediction, match);
        target.Record(prediction, match, combined);
        SymbolRecordings.Append(new SymbolAttempt { label = target.Label, drawer = drawer, points = samples.ToArray() });

        ShowResult(prediction, match, combined);
        ShowTargetStats();
        ShowTotals();
    }

    private async UniTaskVoid Replay()
    {
        isReplaying = true;
        isDrawing = false;

        foreach (SymbolCell cell in cells)
            cell.Clear();

        SymbolAttempt[] attempts = SymbolRecordings.Load()
            .Where(attempt => SymbolRecognizer.Active.Contains(attempt.label))
            .ToArray();

        await BuildTemplates(attempts);

        for (int i = 0; i < attempts.Length; i++)
        {
            Vector2[] points = Trace(attempts[i]);
            SymbolPrediction prediction = recognizer.Predict(rig.Primary, visibleLine);
            TemplateMatch match = templates.Recognize(points, i);
            cells.First(cell => cell.Label == attempts[i].label).Record(prediction, match, new CombinedMatch(prediction, match));

            if (i % 10 == 9)
            {
                totals.text = $"re-scoring recordings  {i + 1}/{attempts.Length}";
                await UniTask.Yield(destroyCancellationToken);
            }
        }

        isReplaying = false;
        stroke.Clear();

        ClearPrediction();
        ShowTargetStats();
        ShowTotals();
    }

    private async UniTask BuildTemplates(SymbolAttempt[] attempts)
    {
        templates.Clear();
        built.Clear();

        IGrouping<string, int>[] groups = Enumerable.Range(0, attempts.Length)
            .GroupBy(i => attempts[i].label)
            .ToArray();

        for (int g = 0; g < groups.Length; g++)
        {
            int[] sources = groups[g].ToArray();
            Vector2[][] clouds = sources.Select(i => PointCloudRecognizer.Normalize(Trace(attempts[i]))).ToArray();

            foreach (int pick in TemplateSelection.Pick(clouds))
            {
                templates.Add(groups[g].Key, clouds[pick], sources[pick]);
                built.Add(new SymbolTemplates.Entry { label = groups[g].Key, cloud = clouds[pick] });
            }

            totals.text = $"building templates  {g + 1}/{groups.Length}";
            await UniTask.Yield(destroyCancellationToken);
        }
    }

    private Vector2[] Trace(SymbolAttempt attempt)
    {
        stroke.Begin(settings.minPointWorldDistance, float.PositiveInfinity);
        foreach (Vector2 point in attempt.points)
            stroke.AddPoint(point);

        return PointCloudRecognizer.Points(rig.Primary);
    }

    private void SaveTemplates()
    {
#if UNITY_EDITOR
        settings.Templates.Entries.Clear();
        settings.Templates.Entries.AddRange(built);
        UnityEditor.EditorUtility.SetDirty(settings.Templates);
        UnityEditor.AssetDatabase.SaveAssets();

        verdict.text = $"<color=#6fdc8c>saved</color> <size=60%>{built.Count} templates to {settings.Templates.name}, the game uses them on next play</size>";
#endif
    }

    private Vector3 ScreenToWorld(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawZone, screenPosition, null, out Vector2 local);

        Rect zone = drawZone.rect;

        return displayCamera.ViewportToWorldPoint(new Vector3(
            Mathf.Clamp01((local.x - zone.xMin) / zone.width),
            Mathf.Clamp01((local.y - zone.yMin) / zone.height),
            Mathf.Abs(displayCamera.transform.position.z)));
    }

    private void Select(int position)
    {
        EventSystem.current.SetSelectedGameObject(null);

        target = cells[position];
        foreach (SymbolCell cell in cells)
            cell.SetSelected(cell == target);

        targetGlyph.text = target.Glyph;
        targetLabel.text = target.Label;
        guide.text = target.Glyph;

        isDrawing = false;
        stroke.Clear();

        ClearPrediction();
        ShowTargetStats();
    }

    private void SetView(SymbolEngine engine)
    {
        view = engine;

        viewTitle.text = $"Symbols  <size=70%><color=#8a8f99>{Name(engine)}   (Tab for {Name(Next(engine))})</color></size>";

        foreach (SymbolCell cell in cells)
            cell.Show(engine);

        ShowGuesses();
        ShowTargetStats();
        ShowTotals();
    }

    private int RandomOtherPosition(int position)
    {
        int other = UnityEngine.Random.Range(0, cells.Length - 1);
        return other >= position ? other + 1 : other;
    }

    private void ResetStats()
    {
        foreach (SymbolCell cell in cells)
            cell.Clear();

        ClearPrediction();
        ShowTargetStats();
        ShowTotals();
    }

    private void ShowResult(SymbolPrediction prediction, TemplateMatch match, CombinedMatch combined)
    {
        lastPrediction = prediction;
        lastMatch = match;
        lastCombined = combined;
        hasResult = true;

        ShowGuesses();

        verdict.text =
            $"{Outcome(SymbolEngine.Model, prediction.Index, prediction.IsConfident)}\n" +
            $"{Detail($"{Percent(prediction.Probability)} sure, needs {Percent(SymbolRecognizer.Threshold)}")}\n" +
            $"{Outcome(SymbolEngine.Templates, match.Index, match.IsAccepted)}\n" +
            $"{Detail($"distance {match.Distance:F2}, needs <= {PointCloudRecognizer.Threshold:F1}")}\n" +
            $"{Outcome(SymbolEngine.Combined, combined.Index, combined.IsAccepted)}\n" +
            $"{Detail($"{Percent(combined.Probability)} / {combined.Distance:F2}, needs {Percent(CombinedMatch.Threshold)} / {CombinedMatch.MaxDistance:F1}")}";
    }

    private static string Detail(string text)
    {
        return $"<size=55%><color=#8a8f99>{text}</color></size>";
    }

    private string Outcome(SymbolEngine engine, int guess, bool accepted)
    {
        string glyph = Glyphs.LatexToUnicode[SymbolRecognizer.Labels[guess]];
        string result = !accepted
            ? $"<color=#ffb347>REJECTED</color>  <size=70%>best {glyph}</size>"
            : guess == target.Index
                ? "<color=#6fdc8c>HIT</color>"
                : $"<color=#ff6b6b>MISS</color>  <size=70%>read as {glyph}</size>";

        return $"<size=60%>{Name(engine)}</size>  {result}";
    }

    private void ShowGuesses()
    {
        if (!hasResult)
        {
            foreach (PredictionRow row in rows)
                row.Hide();

            return;
        }

        IEnumerable<int> indices = cells.Select(cell => cell.Index);
        int[] ranked = view switch
        {
            SymbolEngine.Model => indices.OrderByDescending(i => lastPrediction.Probabilities[i]).ToArray(),
            SymbolEngine.Templates => indices.OrderBy(i => lastMatch.Distances[i]).ToArray(),
            _ => indices.OrderByDescending(i => lastCombined.Probabilities[i]).ToArray()
        };

        for (int i = 0; i < rows.Length; i++)
        {
            int index = ranked[i];
            bool isTarget = index == target.Index;

            switch (view)
            {
                case SymbolEngine.Model:
                    rows[i].Show(index, lastPrediction.Probabilities[index],
                        $"{Percent(lastPrediction.Probabilities[index])}  <color=#8a8f99><size=75%>{lastPrediction.Scores[index]:F1}</size></color>",
                        isTarget);
                    break;
                case SymbolEngine.Templates:
                    rows[i].Show(index, 1f - lastMatch.Distances[index] / (2f * PointCloudRecognizer.Threshold),
                        $"{lastMatch.Distances[index]:F2}",
                        isTarget);
                    break;
                default:
                    rows[i].Show(index, lastCombined.Probabilities[index],
                        $"{Percent(lastCombined.Probabilities[index])}  <color=#8a8f99><size=75%>{lastMatch.Distances[index]:F2}</size></color>",
                        isTarget);
                    break;
            }
        }
    }

    private void ClearPrediction()
    {
        hasResult = false;
        ShowGuesses();
        verdict.text = "";
    }

    private void ShowTargetStats()
    {
        SymbolScore score = target.Score(view);

        if (score.Attempts == 0)
        {
            targetStats.text = "not tried yet";
            return;
        }

        string confusions = string.Join("   ", score.Confusions().Take(3)
            .Select(entry => $"{Glyphs.LatexToUnicode[SymbolRecognizer.Labels[entry.Key]]} x{entry.Value}"));

        targetStats.text = $"<color=#8a8f99>{Name(view)}</color>  {score.Hits}/{score.Attempts} hits  ({Percent((float)score.Hits / score.Attempts)})";

        if (score.Rejected > 0)
            targetStats.text += $"   <color=#ffb347>{score.Rejected} rejected</color>";

        if (confusions.Length > 0)
            targetStats.text += $"\n<color=#8a8f99>misread as</color>  {confusions}";
    }

    private void ShowTotals()
    {
        totals.text = cells.Sum(cell => cell.Score(SymbolEngine.Model).Attempts) == 0
            ? "no attempts yet"
            : $"{Total(SymbolEngine.Model)}\n{Total(SymbolEngine.Templates)}\n{Total(SymbolEngine.Combined)}";
    }

    private string Total(SymbolEngine engine)
    {
        int attempts = cells.Sum(cell => cell.Score(engine).Attempts);
        int hits = cells.Sum(cell => cell.Score(engine).Hits);
        int rejected = cells.Sum(cell => cell.Score(engine).Rejected);

        return $"{Name(engine)}  {hits}/{attempts} ({Percent((float)hits / attempts)})  " +
               $"<color=#ffb347>{rejected} rejected</color>  <color=#ff6b6b>{attempts - hits - rejected} wrong</color>";
    }

    private static string Name(SymbolEngine engine)
    {
        return engine switch
        {
            SymbolEngine.Model => "model",
            SymbolEngine.Templates => "$P",
            _ => "combined"
        };
    }

    private static SymbolEngine Next(SymbolEngine engine)
    {
        return (SymbolEngine)(((int)engine + 1) % Enum.GetValues(typeof(SymbolEngine)).Length);
    }

    private static string Percent(float value)
    {
        return $"{value * 100f:F0}%";
    }
}
