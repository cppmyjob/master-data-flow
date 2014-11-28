using MasterDataFlow.Intelligence.Genetic.Old;

namespace MasterDataFlow.Intelligence.Genetic
{
	public abstract class MoveGeneticManager : GeneticManager
	{
		protected MoveGeneticManager(int itemsCount, int surviveCount, int valuesCount) :
			base(itemsCount, surviveCount, valuesCount) { }


		//private static int _selector = 0;


		/*
				protected internal override GeneticItem CreateChild(GeneticItem fp, GeneticItem sp)
				{
					//++_selector;
					//if ((_selector & 1) == 0)
						//return base.CreateChild(fp, sp);

					MoveGeneticItem firstParent = (MoveGeneticItem)fp;
					MoveGeneticItem secondParent = (MoveGeneticItem)sp;

					MoveGeneticItem child = (MoveGeneticItem)InternalCreateItem();

					// Определяем количество частей для обмена
					int partCount = _random.Next(_valuesCount) + 1;
					float coeff = (partCount / (float)_valuesCount);
					// Признак того, кто будет первым при копировании 1 или 2 предок
					bool isFirst = _random.Next(10) < 5;
					int lastPartIndex = 0;

					double[] firstSteps = firstParent.Steps;
					double[] secondSteps = secondParent.Steps;
					double[] childSteps = child.Steps;

					//MoveDirection[] firstDirections = firstParent.Directions;
					//MoveDirection[] secondDirections = secondParent.Directions;
					//MoveDirection[] childDirections = child.Directions;


					// Это признак того, что-бы копировать одного предка относительно 
					// другого со смещением
					int secondOffset = 0;
					if (_random.NextDouble() > 0.999)
						//if (_random.NextDouble() > 0.9)
						secondOffset = _random.Next(_valuesCount) + 1;
					// 
					for (int i = 0; i < _valuesCount; i++)
					{
						double step;
		//				MoveDirection direction;
						if (secondOffset == 0)
						{
							step = isFirst ? firstSteps[i] : secondSteps[i];
		//					direction = isFirst ? firstDirections[i] : secondDirections[i];
						}
						else
						{
							if (isFirst)
							{
								step = firstSteps[i];
		//						direction = firstDirections[i];
							}
							else
							{
								if (secondOffset + i < _valuesCount)
								{
									step = secondSteps[secondOffset + i];
		//							direction = secondDirections[secondOffset + i];
								}
								else
								{
									step = child.CreateStep(_random.NextDouble());
		//							direction = child.CreateDirection(_random.NextDouble());
								}
							}
						}

						// Это мутация значения
						if (_random.NextDouble() > 0.999)
						//                if (_random.NextDouble() > 0.9)
						{
							double stepStep = child.CreateStep(_random.NextDouble());
							step = stepStep;
		//					MoveDirection directionDirection = child.CreateDirection(_random.NextDouble());
		//					direction = directionDirection;
						}
						childSteps[i] = step;
		//				childDirections[i] = direction;
						int partIndex = (int)(coeff * i);
						if (partIndex != lastPartIndex)
						{
							isFirst = !isFirst;
							lastPartIndex = partIndex;
						}
					}

					if (secondParent.InitData.YearOfBorn < firstParent.InitData.YearOfBorn)
						child.CopyValues(secondParent);
					else
						child.CopyValues(firstParent);
					child.MoveValues();
					return child;
				}
		 */

		protected override double InternalCalculateFitness(GeneticItem item, int processor)
		{
			double lastFitness = CalculateFitness(item, processor);
			while (true)
			{
				((MoveGeneticItem)item).SaveValues();
				((MoveGeneticItem)item).MoveValues();

				double fitness = CalculateFitness(item, processor);
				if (fitness <= lastFitness)
				{
					((MoveGeneticItem)item).RestoreValues();
					return lastFitness;
				}
				lastFitness = fitness;
			}
		}


		protected internal override GeneticItem CreateChild(GeneticItem fp, GeneticItem sp)
		{
			//++_selector;
			//if ((_selector & 1) == 0)
			//return base.CreateChild(fp, sp);

			MoveGeneticItem firstParent = (MoveGeneticItem)fp;
			MoveGeneticItem secondParent = (MoveGeneticItem)sp;

			MoveGeneticItem child = (MoveGeneticItem)InternalCreateItem();

			// Определяем количество частей для обмена
			int partCount = _random.Next(_valuesCount) + 1;
			float coeff = (partCount / (float)_valuesCount);
			// Признак того, кто будет первым при копировании 1 или 2 предок
			bool isFirst = _random.Next(10) < 5;
			int lastPartIndex = 0;

			double[] firstSteps = firstParent.Steps;
			double[] secondSteps = secondParent.Steps;
			double[] childSteps = child.Steps;

			double[] firstValues = firstParent.Values;
			double[] secondValues = secondParent.Values;
			double[] childValues = child.Values;


			// Это признак того, что-бы копировать одного предка относительно 
			// другого со смещением
			int secondOffset = 0;
			if (_random.NextDouble() > 0.999)
				//if (_random.NextDouble() > 0.9)
				secondOffset = _random.Next(_valuesCount) + 1;
			// 
			for (int i = 0; i < _valuesCount; i++)
			{
				double step;
				double value;
				if (secondOffset == 0)
				{
					step = isFirst ? firstSteps[i] : secondSteps[i];
					value = isFirst ? firstValues[i] : secondValues[i];
				}
				else
				{
					if (isFirst)
					{
						step = firstSteps[i];
						value = firstValues[i];
					}
					else
					{
						if (secondOffset + i < _valuesCount)
						{
							step = secondSteps[secondOffset + i];
							value = secondValues[secondOffset + i];
						}
						else
						{
							step = child.CreateStep(_random.NextDouble());
							value = child.CreateValue(_random.NextDouble());
						}
					}
				}

				// Это мутация значения
				if (_random.NextDouble() > 0.999)
				//                if (_random.NextDouble() > 0.9)
				{
					double stepStep = child.CreateStep(_random.NextDouble());
					step = stepStep;
					double valueValue = child.CreateValue(_random.NextDouble());
					value = valueValue;
				}
				childSteps[i] = step;
				childValues[i] = value;
				int partIndex = (int)(coeff * i);
				if (partIndex != lastPartIndex)
				{
					isFirst = !isFirst;
					lastPartIndex = partIndex;
				}
			}

//			if (secondParent.InitData.YearOfBorn < firstParent.InitData.YearOfBorn)
//				child.CopyValues(secondParent);
//			else
//				child.CopyValues(firstParent);
//			child.MoveValues();
			return child;
		}
	}
}
