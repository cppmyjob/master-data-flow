using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Intelligence.Neuron.Dna;

namespace MasterDataFlow.Intelligence.Neuron
{

    public class AxonInstance
    {
        private readonly DnaAxon _axon;

        public AxonInstance(DnaAxon axon)
        {
            _axon = axon;
        }

        public DnaAxon Axon
        {
            get { return _axon; }
        }

        public float Value { get; set; }
    }

    public abstract class InOutInstance
    {
        private readonly DnaInOut _dnaInOut;
        private readonly AxonInstance[] _inputs;
        private readonly AxonInstance[] _outputs;

        protected InOutInstance(DnaInOut dnaInOut)
        {
            _dnaInOut = dnaInOut;
            _inputs = dnaInOut.Inputs.Select(t => new AxonInstance(t)).ToArray();
            _outputs = dnaInOut.Outputs.Select(t => new AxonInstance(t)).ToArray();
        }

        public AxonInstance[] Inputs
        {
            get { return _inputs; }
        }

        public AxonInstance[] Outputs
        {
            get { return _outputs; }
        }

    }

    public abstract class InOutMoveInstance : InOutInstance
    {
        private class Move
        {
            public AxonInstance In { get; set; }
            public AxonInstance Out { get; set; }
        }

        private readonly List<Move> _inputMoves = new List<Move>();
        private readonly List<Move> _outputMoves = new List<Move>();

        protected InOutMoveInstance(DnaInOut dnaInOut) : base(dnaInOut)
        {
        }

        public virtual void Build()
        {
            BuildInputMoves();
            BuildOutputMoves();
        }


        protected internal virtual void MoveInput()
        {
            foreach (var move in _inputMoves)
            {
                move.Out.Value = move.In.Value;
            }
        }

        protected internal virtual void MoveOutput()
        {
            foreach (var move in _outputMoves)
            {
                move.Out.Value = move.In.Value;
            }
        }

        private void BuildOutputMoves()
        {
            var allPoints = Outputs.ToList();
            foreach (var output in GetInternalOutputAxons())
            {
                foreach (var index in output.Axon.Indexes)
                {
                    var inputPoint = GetInputPoints(index, allPoints);
                    var move = new Move()
                    {
                        In = output,
                        Out = inputPoint
                    };
                    _outputMoves.Add(move);
                }
            }
        }

        private void BuildInputMoves()
        {
            var allPoints = GetInternalInputAxons();
            foreach (var input in Inputs)
            {
                foreach (var index in input.Axon.Indexes)
                {
                    var inputPoint = GetInputPoints(index, allPoints);
                    var move = new Move()
                    {
                        In = input,
                        Out = inputPoint
                    };
                    _inputMoves.Add(move);
                }
            }
        }

        private AxonInstance GetInputPoints(int index, IList<AxonInstance> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Axon.Id == index)
                {
                    var result = points[i];
                    points.RemoveAt(i);
                    return result;
                }
            }
            return null;
        }

        protected abstract IList<AxonInstance> GetInternalInputAxons();
        protected abstract IList<AxonInstance> GetInternalOutputAxons();
    }

    public class AtomMoveInstance : InOutInstance
    {
        private readonly DnaAtom _atomDefinition;
        private readonly INeuronAtom<float> _atom;
        public AtomMoveInstance(DnaAtom atomDefinition) : base(atomDefinition)
        {
            _atomDefinition = atomDefinition;
            _atom = (INeuronAtom<float>)Activator.CreateInstance(_atomDefinition.AtomType);
        }

        public void Compute()
        {
            var inputs = Inputs.Select(t => t.Value).ToArray();
            var outputs = _atom.NetworkCompute(inputs);
            for (int i = 0; i < outputs.Length; i++)
            {
                Outputs[i].Value = outputs[i];
            }
        }
    }

    public class SectionMoveInstance : InOutMoveInstance
    {
        private readonly AtomMoveInstance[] _atoms;
        private readonly DnaSection _section;

        public SectionMoveInstance(DnaSection section) : base(section)
        {
            _section = section;
            _atoms = section.Definitions.Select(t => new AtomMoveInstance(t)).ToArray();
        }

        public DnaSection Section
        {
            get { return _section; }
        }

        public AtomMoveInstance[] Atoms
        {
            get { return _atoms; }
        }

        //protected internal override void MoveOutput()
        //{
        //    foreach (var atom in _atoms)
        //    {
        //        sectionInstance.MoveOutput();
        //    }
        //    base.MoveOutput();
        //}

        protected override IList<AxonInstance> GetInternalInputAxons()
        {
            var result = _atoms.SelectMany(t => t.Inputs).ToList();
            return result;
        }

        protected override IList<AxonInstance> GetInternalOutputAxons()
        {
            var result = _atoms.SelectMany(t => t.Outputs).ToList();
            return result;
        }

    }

    public class NetworkInstance : InOutMoveInstance
    {
        private readonly SectionMoveInstance[] _sections;

        public NetworkInstance(Dna.Dna dna) : base(dna)
        {
            _sections = dna.Sections.Select(t => new SectionMoveInstance(t)).ToArray();
        }

        public SectionMoveInstance[] Sections
        {
            get { return _sections; }
        }

        public override void Build()
        {
            base.Build();
            foreach (var section in _sections)
            {
                section.Build();
            }
        }

        public void Compute(float[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                Inputs[i].Value = input[i];
            }
            MoveInput();
            foreach (var section in _sections)
            {
                foreach (var atom in section.Atoms)
                {
                    atom.Compute();
                }
            }
            MoveOutput();
        }

        protected internal override void MoveOutput()
        {
            foreach (var sectionInstance in _sections)
            {
                sectionInstance.MoveOutput();
            }
            base.MoveOutput();
        }

        // -------- --------------

        protected internal override void MoveInput()
        {
            base.MoveInput();
            foreach (var sectionInstance in _sections)
            {
                sectionInstance.MoveInput();
            }
        }

        protected override IList<AxonInstance> GetInternalInputAxons()
        {
            var result = _sections.SelectMany(t => t.Inputs).ToList();
            return result;
        }

        protected override IList<AxonInstance> GetInternalOutputAxons()
        {
            var result = _sections.SelectMany(t => t.Outputs).ToList();
            return result;
        }
    }
}
