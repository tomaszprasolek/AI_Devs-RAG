# AI_Devs-RAG

Simple RAG implementation for [aidevs.pl](https://aidevs.pl) course content.

## Technologies used

- **[OpenAI](https://platform.openai.com/docs/api-reference)** API. Used models:
  -  **text-embedding-ada-002** to generate embeddings.
  -   **gpt-3.5-turbo-16k** for re-rank result obtained from vector database.
  -  **gpt-3.5-turbo-0125** to get final answer.
- **[Qdrant](https://qdrant.tech)** - vector database to store embeddings.
- **C#** to write console app to ask questions 😊

## How to run

- Run [Qdrant](https://qdrant.tech) locally
- Configure settings in the file [appsettings.json](AiDevsRag/appsettings.json)
  - Set Qdrant base URL.
  - Set Qdrant collection name - in this collection will be stored all embeddings.
  - Set Open AI API key: https://platform.openai.com/docs/api-reference/api-keys.
 - All documents which must be imported, copy to the folder **Memories**.
 - Set `ImportDocuments` to true for the first run.
 - Run the app and ask questions.
 - Enjoy 😊

**Course content is not included in the repository because it's not my property. 😊  
It belongs to guys who prepare the course.**

Some code is taken from this [repository](https://github.com/i-am-alice/2nd-devs/tree/main/chat) and converted to C# code.
