using Azure.AI.Inference;
using Azure.Identity;
using Azure.Core;

var endpoint = Environment.GetEnvironmentVariable("AZURE_AI_CHAT_ENDPOINT")
    ?? throw new InvalidOperationException("Set AZURE_AI_CHAT_ENDPOINT environment variable.");

var model = Environment.GetEnvironmentVariable("AZURE_AI_MODEL") ?? "cohere-command-a-plus";

// DefaultAzureCredential handles Entra ID auth — no API key in code.
// Sign in first with: az login
var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
{
    ExcludeManagedIdentityCredential = true
});

// Foundry (Cognitive Services) endpoints require a token whose audience is
// https://cognitiveservices.azure.com. Force that scope regardless of the
// scope the Azure.AI.Inference SDK requests by default.
var scopedCredential = new ScopeOverrideCredential(
    credential, "https://cognitiveservices.azure.com/.default");

var client = new ChatCompletionsClient(new Uri(endpoint), scopedCredential);

var options = new ChatCompletionsOptions
{
    Model = model,
    Temperature = 0.1f,
    MaxTokens = 2000
};

options.Messages.Add(new ChatRequestSystemMessage(
    """
    You are a senior contracts analyst reviewing enterprise agreements.
 
    Your job is to extract key terms and flag risks. Be precise and cite the exact
    contract language when identifying issues. If a required clause is missing,
    say so explicitly rather than guessing at its content.
 
    Return your analysis as structured JSON with these fields:
    -parties: array of parties to the agreement
    - effective_date: string or "not specified"
    - term_length: string or "not specified"
    - renewal_terms: string or "not specified"
    - key_obligations: array of strings
    - liability_cap: string or "not specified"
    - termination_clauses: array of strings
    - risks: array of { severity: "high"|"medium"|"low", description: string}
    """));

options.Messages.Add(new ChatRequestUserMessage(
    """
    Analyze the following service agreement excerpt:
 
    ---
    SOFTWARE LICENSING AGREEMENT
 
    This Agreement is entered into on 1 August 2026 between Zeytin Software Ltd
    ("Licensor") and GlobalCorp Industries GmbH ("Licensee").
 
    1.LICENSE GRANT.Licensor grants Licensee a non-exclusive, non-transferable
       license to use the Software during the Term.
 
    2. TERM. This Agreement shall commence on the Effective Date and continue
       for an initial period of three (3) years, automatically renewing for
       successive one (1) year periods unless either party provides ninety (90)
       days written notice of non-renewal.
 
    3. FEES. Licensee shall pay an annual license fee of EUR 250,000.
 
    4. LIABILITY. IN NO EVENT SHALL LICENSOR'S AGGREGATE LIABILITY EXCEED THE
       FEES PAID BY LICENSEE IN THE TWELVE (12) MONTHS PRECEDING THE CLAIM.
 
    5. TERMINATION. Either party may terminate this Agreement for material
       breach if such breach is not cured within thirty (30) days of written
       notice.
    ---
    """));

Console.WriteLine($"Calling model: {model}");
Console.WriteLine("---");

var response = await client.CompleteAsync(options);

Console.WriteLine(response.Value.Content);

options.Messages.Clear();

Console.WriteLine($"Calling Multilingual: {model}");

options.Messages.Add(new ChatRequestSystemMessage(
    """
    You are a senior contracts analyst. Return your analysis in English JSON
    even if the source contract is in another language.
    """));

options.Messages.Add(new ChatRequestUserMessage(
    """
    Analysiere den folgenden Vertragsauszug:
 
    ---
    DIENSTLEISTUNGSVERTRAG


    Dieser Vertrag wird zwischen der Zeytin Software GmbH (&quot; Auftragnehmer & quot;)
    und der Berliner Handelsgesellschaft mbH (&quot; Auftraggeber & quot;) geschlossen.
 
    § 1 VERTRAGSGEGENSTAND.Der Auftragnehmer verpflichtet sich zur Bereitstellung
    von Softwareentwicklungsleistungen.
 
    § 2 LAUFZEIT. Der Vertrag beginnt am 15. September 2026 und läuft für eine
    Erstlaufzeit von zwei (2) Jahren.
 
    § 3 VERGÜTUNG. Der Auftraggeber zahlt einen monatlichen Betrag von EUR 18.500
    zzgl. gesetzlicher Umsatzsteuer.
 
    § 4 KÜNDIGUNG. Der Vertrag kann von beiden Seiten mit einer Frist von drei
    Monaten zum Ende eines Kalenderjahres gekündigt werden.
    ---
    """));

response = await client.CompleteAsync(options);
Console.WriteLine(response.Value.Content);


// Wraps a TokenCredential and forces a fixed audience/scope, ignoring the
// scope the client pipeline asks for.
sealed class ScopeOverrideCredential : TokenCredential
{
    private readonly TokenCredential _inner;
    private readonly string[] _scopes;

    public ScopeOverrideCredential(TokenCredential inner, string scope)
    {
        _inner = inner;
        _scopes = new[] { scope };
    }

    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        => _inner.GetToken(new TokenRequestContext(_scopes), cancellationToken);

    public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        => _inner.GetTokenAsync(new TokenRequestContext(_scopes), cancellationToken);
}