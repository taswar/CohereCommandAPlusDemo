# Cohere Command A+ on Microsoft Foundry — C# Demo

A minimal .NET console app that calls the **Cohere Command A+** model deployed on
**Microsoft Foundry** using the `Azure.AI.Inference` SDK. It demonstrates:

- Passwordless authentication with **Entra ID** (`DefaultAzureCredential`) — no API keys in code.
- A structured-output **contract analysis** prompt that returns JSON.
- A **multilingual** prompt (German contract → English JSON) using the same model.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (the project targets `net10.0`).
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) for sign-in.
- A **Microsoft Foundry** resource with the `cohere-command-a-plus` model deployed.
- Your identity must have the **Cognitive Services User** role on the Foundry resource.

## NuGet packages

The project references:

| Package | Version |
| --- | --- |
| `Azure.AI.Inference` | `1.0.0-beta.5` |
| `Azure.Identity` | `1.21.0` |

These are restored automatically on build.

## Authentication

The app uses `DefaultAzureCredential` and forces the token audience to
`https://cognitiveservices.azure.com/.default` (required for Foundry / Cognitive
Services endpoints). Managed identity is excluded so local runs use your Azure CLI login.

Sign in before running:

```powershell
az login
```

If your identity lacks access, ask an owner to assign the role:

```powershell
az role assignment create `
  --assignee "<your-user-or-object-id>" `
  --role "Cognitive Services User" `
  --scope "<foundry-resource-id>"
```

## Configuration

The app reads two environment variables:

| Variable | Required | Description |
| --- | --- | --- |
| `AZURE_AI_CHAT_ENDPOINT` | Yes | The Foundry inference endpoint URL. |
| `AZURE_AI_MODEL` | No | Model/deployment name. Defaults to `cohere-command-a-plus`. |

Set them for the current PowerShell session:

```powershell
$env:AZURE_AI_CHAT_ENDPOINT = "https://<your-foundry-resource>.services.ai.azure.com/models"
$env:AZURE_AI_MODEL = "cohere-command-a-plus"
```

> The exact endpoint format depends on your Foundry deployment. Copy it from the
> deployment's **Endpoint** field in the Foundry / Azure AI portal.

## Run the demo

From the project folder:

```powershell
cd CohereCommandAPlusDemo
dotnet run
```

Or from the solution root, targeting the project file:

```powershell
dotnet run --project CohereCommandAPlusDemo/CohereCommandAPlusDemo.csproj
```

## What it does

1. **English contract analysis** — sends a software licensing agreement excerpt and
   asks the model to return structured JSON (parties, term, liability cap, risks, etc.).
2. **Multilingual analysis** — clears the conversation, then sends a German service
   agreement and asks for the analysis back in **English JSON**.

Both responses are printed to the console.

## Troubleshooting

| Symptom | Fix |
| --- | --- |
| `Set AZURE_AI_CHAT_ENDPOINT environment variable.` | Set the endpoint variable (see [Configuration](#configuration)). |
| `401 / 403` authorization errors | Run `az login`; ensure you have the **Cognitive Services User** role on the resource. |
| `404` model not found | Verify `AZURE_AI_MODEL` matches your deployment name. |
| Token audience errors | The app already forces the `cognitiveservices.azure.com` scope — confirm the endpoint is a Foundry/Cognitive Services endpoint. |

## Project structure

```
CohereCommandAPlusDemo/
├── CohereCommandAPlusDemo.csproj
├── Program.cs
└── README.md
```
