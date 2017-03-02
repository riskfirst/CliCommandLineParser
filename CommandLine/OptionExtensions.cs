using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine
{
    public static class OptionExtensions
    {
        internal static IEnumerable<Command> AllCommands(
            this Command command)
        {
            if (command == null)
            {
               yield break;
            }

            yield return command;

            foreach (var item in command
                .DefinedOptions
                .FlattenBreadthFirst(o => o.DefinedOptions)
                .OfType<Command>())
            {
                yield return item;
            }
        }

        internal static IEnumerable<Option> FlattenBreadthFirst(
            this IEnumerable<Option> options)
        {
            foreach (var item in options.FlattenBreadthFirst(o => o.DefinedOptions))
            {
                yield return item;
            }
        }

        public static Command Command(this Option option) =>
            option.RecurseWhileNotNull(o => o.Parent)
                  .OfType<Command>()
                  .FirstOrDefault();

        public static bool IsHidden(this Option  option) => 
            string.IsNullOrWhiteSpace(option.HelpText);

        public static string FullyQualifiedName(this Command command) =>
            string.Join(" ",
                        command.RecurseWhileNotNull(c => c.Parent as Command)
                               .Reverse());

        internal static IEnumerable<AppliedOption> AllOptions(
            this AppliedOption option)
        {
            if (option == null)
            {
                yield break;
            }

            yield return option;

            foreach (var item in option.AppliedOptions.FlattenBreadthFirst(o => o.AppliedOptions))
            {
                yield return item;
            }
        }

        public static ParseResult Parse(
            this Option option,
            params string[] args) =>
            new Parser(option).Parse(args);

        public static ParseResult Parse(
            this Option option,
            string commandLine) =>
            new Parser(option).Parse(commandLine);
    }
}