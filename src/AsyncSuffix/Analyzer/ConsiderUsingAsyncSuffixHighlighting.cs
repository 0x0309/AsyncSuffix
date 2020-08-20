using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using Sizikov.AsyncSuffix.Analyzer;


namespace Sizikov.AsyncSuffix.Analyzer
{
    [RegisterConfigurableSeverity(SeverityId,
        null,
        HighlightingGroupIds.BestPractice,
        "Consider adding Async suffix",
        "According to Microsoft guidelines a method which is Task-returning and is asynchronous in nature should have an 'Async' suffix. ",
        Severity.SUGGESTION)]
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.WARNING)]
    public sealed class ConsiderUsingAsyncSuffixHighlighting : IHighlighting
    {
        private const string SeverityId = "ConsiderUsingAsyncSuffix";

        public IMethodDeclaration MethodDeclaration { get; private set; }

        public ConsiderUsingAsyncSuffixHighlighting(IMethodDeclaration methodDeclaration)
        {
            MethodDeclaration = methodDeclaration;
        }

        public DocumentRange CalculateRange() => MethodDeclaration.NameIdentifier.GetDocumentRange();

        public string ToolTip => "Async method name does not have 'Async' suffix";

        public string ErrorStripeToolTip => ToolTip;

        public int NavigationOffsetPatch => 0;

        public bool IsValid() => MethodDeclaration == null || MethodDeclaration.IsValid();
    }
}