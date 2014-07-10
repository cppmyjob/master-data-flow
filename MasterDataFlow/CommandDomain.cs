﻿using System;
using System.Collections.Generic;
using System.Linq;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow
{
    public delegate void OnNextCommand(BaseCommand command);

    public delegate void OnCommandError(CommandInfo info);

    public class CommandDomain : ICommandDomain
    {
        private readonly IList<CommandDefinition> _definitions = new List<CommandDefinition>();
        private readonly Guid _id;
        private readonly CommandRunner _runner;

        internal CommandDomain(Guid id, CommandRunner runner)
        {
            _id = id;
            _runner = runner;
        }

        public CommandDomain(CommandRunner runner)
        {
            _id = Guid.NewGuid();
            _runner = runner;
        }

        internal IList<CommandDefinition> Definitions
        {
            get { return _definitions; }
        }

        public Guid Id
        {
            get { return _id; }
        }

        public CommandDefinition Find<TCommand>()
            where TCommand : ICommand<ICommandDataObject>
        {
            Type commandType = typeof (TCommand);
            return Find(commandType);
        }

        public void Register(CommandDefinition definition)
        {
            _definitions.Add(definition);
        }

        public Guid Start<TCommand>(ICommandDataObject commandDataObject, EventLoopCallback callback = null)
            where TCommand : ICommand<ICommandDataObject>
        {
            Type commandType = typeof (TCommand);

            CommandDefinition commandDefinition = Find(commandType);
            // TODO check if commandDefinition was found
            return _runner.Run(this, commandDefinition, commandDataObject, callback);
        }

        private CommandDefinition Find(Type commandType)
        {
            CommandDefinition result = _definitions.FirstOrDefault(t => t.Command == commandType);
            return result;
        }
    }
}