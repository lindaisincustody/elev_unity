using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EdenAI;

public class PoemAI : MonoBehaviour
{
    public bool sendRequests = false;
    [SerializeField] BookController bookController;
    private string APIKEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiNzFlMWU3OWMtZDM0NS00YzdhLWIxMjItZGNiMTllMjFlMWFmIiwidHlwZSI6ImFwaV90b2tlbiJ9.To_WQA14fk_VbWcFI77dyU9HP9JFGM-RPqw5BhFs6nM";
    private string prompt = "Please add 4 lines to this poem, ensuring that each line does not begin with the same word as the preceding line, and each line should not exceed 6 words in length. Your added version should be very abstract, not focusing on one emotion. Do not write my part in your poem. The poem is: ";
    public async void SendRequest(WordData wordsData)
    {
        if (!sendRequests)
            return;
        string provider = "openai";
        string text = prompt + wordsData.Poem;
        EdenAIApi edenAI = new EdenAIApi(APIKEY);
        ChatResponse response = await edenAI.SendChatRequest(provider, text);
        bookController.ExtendPoemAI(response.generated_text);
    }
}
