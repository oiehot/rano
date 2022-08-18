#if false

using UnityEngine;

namespace Rano.TutorialSystem
{
    public class OpenPackagePhase : IPhase
    {
        [SerializeField] private TutorialDialogue _dialogue;
        [SerializeField] private TutorialFocus _focus;
        
        private IEnumerator Play()
        {
            _dialogue.Print("HELLO WORLD!?");
            _focus.Focus(EFocusType.Circle, )
        }

        public void Stop()
        {
            
        }
    }
}

#endif