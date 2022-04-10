using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MBW.Tests.SignaturesTester;

public class SignaturesBuilder
{
    private delegate IEnumerable<string> SignaturesFunc(Type type);
    public delegate Type ArgumentEvaluator(Type type);
    public delegate Type[] ArgumentEvaluatorMultiple(Type type);
    public const string SelfType = "TSelf";

    private IList<Type> _types;
    private IList<string> _signatures;
    private IList<SignaturesFunc> _signatureFuncs;

    private SignaturesBuilder(Type[] types)
    {
        _signatures = new List<string>();
        _signatureFuncs = new List<SignaturesFunc>();
        _types = types.ToArray();
    }

    public static SignaturesBuilder Types(params Type[] types) => new SignaturesBuilder(types);
    public static SignaturesBuilder Types<T>() => Types(typeof(T));
    public static SignaturesBuilder Types<T, T1>() => Types(typeof(T), typeof(T1));
    public static SignaturesBuilder Types<T, T1, T2>() => Types(typeof(T), typeof(T1), typeof(T2));

    public SignaturesBuilder MustHave(string signature)
    {
        _signatures.Add(signature);
        return this;
    }

    /// <summary>
    /// Adds a signature with a variable set of arguments, that can change for each Type we're evaluating
    /// 
    /// Ex: MustHave("void A(T1, T2)", [string, int], [ulong, byte]) => 
    /// - void A(string, ulong)
    /// - void A(int, byte)
    /// </summary>
    public SignaturesBuilder MustHave(string signature, params ArgumentEvaluatorMultiple[] argT)
    {
        // Produce N signatures, one for each set of arguments
        _signatureFuncs.Add(type =>
        {
            Type[][] argTypes = argT.Select(s => s(type)).ToArray();
            int argSets = argTypes[0].Length;

            if (argTypes.Any(x => x.Length != argSets))
                throw new ArgumentException($"Signature '{signature}' was given an unequal set of arguments: {string.Join(", ", argTypes.Select(x => x.Length))}");

            string[] resultSignatures = new string[argSets];
            for (int setIdx = 0; setIdx < argTypes.Length; setIdx++)
            {
                string tmpSignature = signature;
                for (int i = 0; i < argTypes.Length; i++)
                {
                    int length = tmpSignature.Length;
                    int idxToReplace = i + 1;

                    Type argType = argTypes[setIdx][i];
                    string argTypeString = GetTypeString(argType, type);

                    tmpSignature = Regex.Replace(tmpSignature, $@"\bT{idxToReplace}\b", argTypeString, RegexOptions.None);

                    if (tmpSignature.Length == length)
                        throw new InvalidOperationException($"Expected to replace T{idxToReplace} in '{signature}' with '{argTypeString}', but found nothing");
                }

                resultSignatures[setIdx] = tmpSignature;
            }

            return resultSignatures;
        });
        return this;
    }

    public IEnumerable<(Type type, string signature)> GetDesiredSignatures()
    {
        foreach (Type type in _types)
        {
            // Yield generic signatures
            foreach (string signature in _signatures)
                yield return (type, signature);

            // Produce signatures for this type specifically
            foreach (SignaturesFunc func in _signatureFuncs)
            {
                IEnumerable<string> signatures = func(type);
                foreach (string signature in signatures)
                    yield return (type, signature);
            }
        }
    }

    private string GetTypeString(Type targetType, Type instanceType)
    {
        if (targetType == instanceType)
            return SelfType;

        if (targetType.IsPrimitive)
            // .NET Primitives are as "String", not "System.String"
            return targetType.Name;

        return targetType.FullName;
    }
}
