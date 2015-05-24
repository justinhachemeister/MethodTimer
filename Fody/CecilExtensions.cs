﻿using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

public static class CecilExtensions
{
    public static IEnumerable<MethodDefinition> AbstractMethods(this TypeDefinition type)
    {
        return type.Methods.Where(x => x.IsAbstract);
    }

    public static IEnumerable<MethodDefinition> ConcreteMethods(this TypeDefinition type)
    {
        return type.Methods.Where(x => !x.IsAbstract && x.HasBody && !IsEmptyConstructor(x));
    }

    static bool IsEmptyConstructor(this MethodDefinition method)
    {
        return ((method.Name == ".ctor") && (method.Body.Instructions.Count == 3));
    }

    public static bool IsInstanceConstructor(this MethodDefinition methodDefinition)
    {
        return methodDefinition.IsConstructor && !methodDefinition.IsStatic;
    }

    public static void InsertBefore(this MethodBody body, Instruction target, Instruction instruction)
    {
        body.Instructions.InsertBefore(target, instruction);
    }

    public static void InsertBefore(this Collection<Instruction> instructions, Instruction target, Instruction instruction)
    {
        var index = instructions.IndexOf(target);
        instructions.Insert(index, instruction);
    }

    public static string MethodName(this MethodDefinition method)
    {
        if (method.IsConstructor)
        {
            return string.Format("{0}{1} ", method.DeclaringType.Name, method.Name);
        }
        return string.Format("{0}.{1} ", method.DeclaringType.Name, method.Name);
    }

    public static void Insert(this MethodBody body, int index, IEnumerable<Instruction> instructions)
    {
        instructions = instructions.Reverse();
        foreach (var instruction in instructions)
        {
            body.Instructions.Insert(index, instruction);
        }
    }

    public static void Add(this MethodBody body, params Instruction[] instructions)
    {
        foreach (var instruction in instructions)
        {
            body.Instructions.Add(instruction);
        }
    }

    public static CustomAttribute GetAsyncStateMachineAttribute(this MethodDefinition method)
    {
        return method.CustomAttributes.FirstOrDefault(_ => _.AttributeType.Name == "AsyncStateMachineAttribute");
    }

    public static bool IsAsync(this MethodDefinition method)
    {
        return GetAsyncStateMachineAttribute(method) != null;
    }

    public static bool IsLeaveInstruction(this Instruction instruction)
    {
        if ((instruction.OpCode == OpCodes.Leave) || (instruction.OpCode == OpCodes.Leave_S))
        {
            return true;
        }

        return false;
    }

}