# AI_Devs-RAG

Simple RAG implementation for [aidevs.pl](https://aidevs.pl) course.

## How to run

- Run [Qdrant](https://qdrant.tech) locally
- Configure settings in the file [appsettings.json](AiDevsRag/appsettings.json)
  - Set Qdrant base URL
  - Set Qdrant collection name - in this collection will be stored all embeddings
  - Set Open AI API key: https://platform.openai.com/docs/api-reference/api-keys
 - All documents which must be imported, copy to the folder **Memories**
 - Set `ImportDocuments` to true for the first run
 - Run the app and ask questions.
 - Enjoy 😊



Some code is taken from this [repository](https://github.com/i-am-alice/2nd-devs/tree/main/chat) and converted to C# code.
