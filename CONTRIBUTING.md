# Contributing

Thanks for helping improve this sample. It's a small .NET 10 console app that calls Cohere Command A+ on Microsoft Foundry with the `Azure.AI.Inference` SDK and Entra ID, and runs two contract analysis prompts: one in English and one in German that answers in English. Contributions that make it clearer, more correct, or more useful to other .NET developers are welcome.

## Ways to contribute

- **Report a bug.** A call fails, or the README doesn't match what the code does.
- **Fix the docs.** Typos, unclear steps, or a missing troubleshooting case (for example, an endpoint format that didn't work for you).
- **Improve the sample.** For example, another language, a longer contract, or parsing the JSON into a C# record.
- **Share results.** If you tried your own contracts or languages, an issue describing what you saw helps others.

For anything larger than a small fix, open an issue first so we can agree on the approach before you write code.

## Set up

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and the Azure CLI.
2. Clone the repo and build it:

   ```bash
   git clone https://github.com/taswar/CohereCommandAPlusDemo.git
   cd CohereCommandAPlusDemo
   dotnet build
   ```

3. To run it, deploy Cohere Command A+ on a Microsoft Foundry resource, set `AZURE_AI_CHAT_ENDPOINT` (and `AZURE_AI_MODEL` if your deployment isn't named `cohere-command-a-plus`), and sign in with `az login`, as described in the [README](README.md#configuration). Your account needs the **Cognitive Services User** role on the resource. The sample uses Entra ID only, with no API key.

## Guidelines

- **Never commit secrets.** No keys, endpoints, tenant IDs or tokens in code, config files or screenshots.
- **Keep the token scope.** Foundry endpoints need a token for `https://cognitiveservices.azure.com/.default`. Keep `ScopeOverrideCredential`, or explain why it's no longer needed.
- **Keep Entra ID.** The sample uses `DefaultAzureCredential` with no API key. Keep it that way.
- **Use made-up contracts.** The sample contracts are fictional. Never add real contracts, names or personal data.
- **Keep the analysis grounded.** The system prompt asks the model to cite contract language and say when a clause is missing. Keep that in any new prompt, and don't present the output as legal advice.
- **Keep it a sample.** The code should stay short and readable in one `Program.cs`. Prefer clarity over abstraction, and add a dependency only when it clearly helps.
- **Match the existing style.** Target `net10.0`, keep nullable reference types on, and follow the layout already in `Program.cs`.
- **Keep the README in sync.** If you add a prompt, change settings or change output, update the README in the same pull request.
- **One change per pull request.** Small, focused pull requests are easier to review and merge.

## Pull request checklist

- [ ] `dotnet build` succeeds with no new warnings.
- [ ] The app runs against your own Command A+ deployment, or you explain why you couldn't run it.
- [ ] No secrets, endpoints or personal data are in the diff.
- [ ] The README reflects any change in prompts, settings or output.
- [ ] The pull request description says what changed and why.

The build workflow runs on every pull request and has to pass before a merge.

## Reporting a bug

Open an issue and include:

- Whether the English or the German call failed, and what you expected.
- The full error message, with any endpoints removed.
- Your .NET SDK version (`dotnet --version`) and operating system.
- Your deployment name and the endpoint format you used (with the resource name removed).

## Security issues

Please don't report security problems in a public issue. Use **Security → Report a vulnerability** on the repo to send a private report.

## License

By contributing, you agree that your contributions are licensed under the [MIT License](LICENSE) that covers this project.
