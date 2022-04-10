using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MBW.Tests.SignaturesTester;

public class TypesMethodStore
{
    private readonly Dictionary<Type, HashSet<string>> _types = new();

    public TypesMethodStore Add(Type type)
    {
        Regex replacer = new Regex("\\b" + Regex.Escape(type.FullName) + "\\b", RegexOptions.Compiled);

        IEnumerable<string> signatures = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(s => s.DeclaringType != typeof(object))
            .Select(s => (s.IsStatic ? "static " : "instance ") + s)
            .Select(s => replacer.Replace(s, SignaturesBuilder.SelfType));

        HashSet<string> methods = new HashSet<string>(StringComparer.Ordinal);
        foreach (string signature in signatures)
        {
            if (!methods.Add(signature))
                throw new InvalidOperationException($"Tried to add duplicate expected signature, '{signature}'");
        }

        _types.Add(type, methods);
        return this;
    }

    public bool HasMethod(Type type, string signature) => _types[type].Contains(signature);
    public string GetClosestMethod(Type type, string signature)
    {
        Fastenshtein.Levenshtein lev = new Fastenshtein.Levenshtein(signature);
        return _types[type].MinBy(s => lev.DistanceFrom(s));
    }
}
