using System.Collections;
using UnityEngine;
using TMPro;
using View.Button;

namespace View.UI.Game
{
    public class StoryScreen : UIScreen
    {
        [SerializeField] private TMP_Text _storyText;
        [SerializeField] private CustomButton _okButton;
        [SerializeField] private float _typeSpeed = 0.04f;

        private Coroutine _typingCoroutine;
        private string[] _stories =
        {
            "The farmer swears he once heard a basket snoring at night.",
            "Yesterday, a chicken stole the farmer’s hat and refused to give it back.",
            "The farmer tried to stack three baskets at once… he’s still looking for the third one.",
            "A mysterious golden fruit appeared on the field, but it vanished when the farmer blinked.",
            "The farmer says the scarecrow moves every night, but no one believes him.",
            "One morning, all baskets were perfectly lined up—no one knows who did it."
        };

        private System.Action _onComplete;

        private void Awake()
        {
            if (_okButton != null)
            {
                _okButton.AddListener(OnOkPressed);
            }
        }

        public void ShowStory(System.Action onComplete)
        {
            _onComplete = onComplete;

            gameObject.SetActive(true);
            _okButton.Interactable = false;
            _okButton.gameObject.SetActive(false);

            _storyText.text = "";
            string randomStory = _stories[Random.Range(0, _stories.Length)];

            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);

            _typingCoroutine = StartCoroutine(TypeRoutine(randomStory));
        }

        private IEnumerator TypeRoutine(string text)
        {
            _storyText.text = "";

            foreach (char c in text)
            {
                _storyText.text += c;
                yield return new WaitForSeconds(_typeSpeed);
            }

            _okButton.gameObject.SetActive(true);
            _okButton.Interactable = true;

            _typingCoroutine = null;
        }

        private void OnOkPressed()
        {
            _okButton.Interactable = false;
            gameObject.SetActive(false);

            _onComplete?.Invoke();
        }
    }
}