using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using Sizikov.AsyncSuffix.Settings;

namespace Sizikov.AsyncSuffix.Analyzer
{
    internal static class AsyncMethodNameUtil
    {
        public static bool IsAsyncSuffixMissing(this IMethodDeclaration methodDeclaration)
        {
            if (methodDeclaration.IsOverride)
                return false;

            var declaredElement = methodDeclaration.DeclaredElement;
            if (declaredElement == null)
                return false;

            if (declaredElement.ShortName.EndsWith("Async", StringComparison.Ordinal))
                return false;

            var memberInstances = declaredElement.GetAllSuperMembers();
            if (memberInstances.Count > 0)
                return false;

            var settings = methodDeclaration.GetSettingsStore();
            var excludeTestMethods = settings.GetValue(AsyncSuffixSettingsAccessor.ExcludeTestMethodsFromAnalysis);
            if (excludeTestMethods)
                if (declaredElement.IsTestMethod() || methodDeclaration.IsAnnotatedWithKnownTestAttribute())
                    return false;

            var returnType = declaredElement.ReturnType as IDeclaredType;
            if (returnType == null) return false;

            if (returnType.IsTaskType())
            {
                if (!declaredElement.IsStatic || declaredElement.ShortName != "Main")
                    return true;

                if (!returnType.IsGenericTask())
                    return false;

                var typeParameter = returnType.GetTypeElement()?.TypeParameters.Single();
                if (typeParameter == null)
                    return true;

                var taskResultType = returnType.Resolve().Substitution.Apply(typeParameter);
                return !taskResultType.IsInt();
            }

            var customAsyncTypeNames = settings.EnumEntryIndices(AsyncSuffixSettingsAccessor.CustomAsyncTypes)
                .ToArray();
            var customAsyncTypes = new List<IDeclaredType>();
            foreach (var type in customAsyncTypeNames)
                customAsyncTypes.Add(TypeFactory.CreateTypeByCLRName(type, declaredElement.Module));

            var returnTypeElement = returnType.GetTypeElement();
            var isCustomAsyncType = returnTypeElement != null && customAsyncTypes.Any(type => returnTypeElement.IsDescendantOf(type.GetTypeElement()));

            return isCustomAsyncType;
        }
    }
}