using System.Collections.Generic;
using System.Data;
using System.Linq;
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

    public class SectionInstance
    {
        private readonly AxonInstance[] _inputs;
        private readonly AxonInstance[] _outputs;
        private readonly DnaSection _section;

        public SectionInstance(DnaSection section)
        {
            _section = section;
            _inputs = section.Inputs.Select(t => new AxonInstance(t)).ToArray();
            _outputs = section.Outputs.Select(t => new AxonInstance(t)).ToArray();
        }

        public DnaSection Section
        {
            get { return _section; }
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

    public class InOutInstance
    {
        private class Move
        {
            public AxonInstance In { get; set; }
            public AxonInstance Out { get; set; }
        }

        private readonly MasterDna _dna;
        private readonly AxonInstance[] _outputs;
        private readonly SectionInstance[] _sections;
        private readonly List<Move> _inputMoves = new List<Move>();
        private readonly List<Move> _outputMoves = new List<Move>();

        protected readonly AxonInstance[] Inputs;


        public InOutInstance(MasterDna dna)
        {
            _dna = dna;
            Inputs = dna.Inputs.Select(t => new AxonInstance(t)).ToArray();
            _outputs = dna.Outputs.Select(t => new AxonInstance(t)).ToArray();
            _sections = dna.Sections.Select(t => new SectionInstance(t)).ToArray();

            Build();
        }

        protected void MoveInput()
        {
            foreach (var move in _inputMoves)
            {
                move.Out.Value = move.In.Value;
            }
        }


        public SectionInstance[] Sections
        {
            get { return _sections; }
        }

        private void Build()
        {
            BuildInputMoves();
            BuildOutputMoves();
        }

        private void BuildOutputMoves()
        {
            var allPoints = GetAllOutputPoints();
            foreach (var output in _sections.SelectMany(t => t.Outputs))
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

        private IList<AxonInstance> GetAllOutputPoints()
        {
            var result = new List<AxonInstance>();
            result.AddRange(_outputs);
            return result;
        }

        private void BuildInputMoves()
        {
            var allPoints = GetAllInputPoints();
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

        private IList<AxonInstance> GetAllInputPoints()
        {
            var result = new List<AxonInstance>();
            //result.AddRange(_inputs);
            result.AddRange(_sections.SelectMany(t => t.Inputs));
            //result.AddRange(_outputs);
            return result;
        }
    }

    public class NetworkInstance : InOutInstance
    {
        public NetworkInstance(MasterDna dna) : base(dna)
        {
        }

        public float[] Compute(float[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                Inputs[i].Value = input[i];
            }
            MoveInput();
            return new[]
            {
                0F
            };
        }
    }
}
