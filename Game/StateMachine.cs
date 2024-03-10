using DobirnaGraServer.Game.State;

namespace DobirnaGraServer.Game
{
	public class StateMachine
	{
		private readonly Dictionary<Type, IState> _states = new();

		private IState? _currentState;
		public IState? CurrentState
		{
			get => _currentState;
			private set
			{
				_currentState?.OnExit();
				_currentState = value;
				_currentState?.OnEnter();
			}
		}

		public void DefineState<T>(T state) 
			where T : IState
		{
			_states.Add(typeof(T), state);
		}

		public void SetInitState<T>()
		{
			Type type = typeof(T);
			if (_states.TryGetValue(type, out IState? state) == false)
				throw new ArgumentException($"{type.Name} is not defined in this state machine!");

			CurrentState = state;
		}

		public void DefineTransition<TFrom, TTo>(Action<TFrom, StateEvent> setupTrigger) 
			where TFrom : IState
			where TTo : IState
		{
			var typeFrom = typeof(TFrom);
			var typeTo = typeof(TTo);

			if (!_states.TryGetValue(typeTo, out IState? to) || to is not TTo toState)
				throw new ArgumentException($"{typeTo.Name} is not defined state for transition!");

			if (!_states.TryGetValue(typeFrom, out IState? from) || from is not TFrom fromState)
				throw new ArgumentException($"{typeFrom.Name} is not defined state for transition!");

			void ActionTransition()
			{
				CurrentState = toState;
			}

			setupTrigger.Invoke(fromState, ActionTransition);
		}
	}
}
