using System;
namespace Holex.StateMachine
{
    public class StateMachine
    {
        private IState _currentState;
        private StateTransition[] _normalTransitions;
        private StateTransition[] _anyTransitions;
        public StateMachine(IState defaultState, StateTransition[] normalTransitions = null, StateTransition[] anyTransitions = null)
        {
            _normalTransitions = normalTransitions;
            _anyTransitions = anyTransitions;
            _currentState = defaultState;
            _currentState.OnEnter(); 
        }
        public void Tick()
        {
            StateTransition transition = GetTransition();
            if (transition is not null)
                SetState(transition.To);  // koþullarý saðlamýþ bir geçiþ var ona geçiyoruz!


            _currentState?.Tick(); 
            _currentState.Tick();
        }

        public void SetState(IState state)
        {
            //State deðiþmemiþse dönüyoruz aksi durumunda OnExit() methodlarý çaðýrýlýr
            if (state == _currentState) return;

            _currentState?.OnExit(); //aktif stateden çýkýlýyor
            _currentState = state; // yeni state atanýyor
            _currentState.OnEnter(); //state enter methodu çaðýrýlýyor

        }
        private StateTransition GetTransition()
        {
            // https://yasirkula.com/2016/06/19/unity-optimizasyon-onerileri/
            // [6] Bir List‘in elemanlarýnýn üzerinden for döngüsü ile geçiyorsanýz ve bu List’in eleman sayýsý for’dayken deðiþmeyecekse,
            // List.Count‘u sadece bir kere çaðýrmaya çalýþýn:

            //for (int i = 0; i < _anyTransitions.Length; i++)
            //sanki iki defa deðiþken tanýmlamamýza gerek yok? -- öncelik sýralamasý için listeyi yukarýdan aþaðý iþlemiz daha doðru buna dokunma!
            if (_anyTransitions is not null)
            {
                for (int i = 0, length = _anyTransitions.Length; i < length; i++)
                {
                    if (_anyTransitions[i].Condition()) return _anyTransitions[i];
                }
            }

            if (_normalTransitions is not null)
            {
                for (int i = 0, length = _normalTransitions.Length; i < length; i++)
                {
                    if (_normalTransitions[i].Condition() && // geçiþ koþulu saðlanmýþ.
                        _normalTransitions[i].From != default &&  // geçiþ yapmak için önceki koþul tanýmlanmýþ mý ona bakýyoruz
                        _currentState == _normalTransitions[i].From) // geçerli state den bu koþula geçilebilir mi?
                    {
                        return _normalTransitions[i]; // herþey yolundaysa geçiþ iþlemini geri gönderiyoruz.
                    }
                }
            }
            return null;
        }
    }

    public class StateTransition
    {
        public Func<bool> Condition { get; }
        public IState From { get; }
        public IState To { get; }

        public StateTransition(IState _from, IState _to, Func<bool> condition)
        {
            To = _to;
            From = _from;
            Condition = condition;
        }
    }

    public interface IState
    {
        void Tick();
        void OnEnter();
        void OnExit();
    }
}