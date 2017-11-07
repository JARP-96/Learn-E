using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
// INTERFAZ PROXY
    public static bool DO_NOT = false;

    [SerializeField]
    private CardState _state;
    [SerializeField]
    private int _cardvalue;
    [SerializeField]
    private bool _initialized = false;

    private Sprite _cardBack;
    private Sprite _cardFace;

    private GameObject _manager;

// FIN INTERFAZ
    void Start()
    {
		_state = CardState.FRONT;
        _manager = GameObject.FindGameObjectWithTag("Manager");
    }
    public void setupGraphics()
    {
        _cardBack = _manager.GetComponent<GameManager>().getCardBack();
        _cardFace = _manager.GetComponent<GameManager>().getCardFace(_cardvalue);

        flipCard();
    }
    public void flipCard()
    {
		if (!DO_NOT) {
			if (_state == CardState.BACK)
				_state = CardState.FRONT;
			else if (_state == CardState.FRONT)
				_state = CardState.BACK;
			if (_state == CardState.BACK)
				GetComponent<Image> ().sprite = _cardBack;
			else if (_state == CardState.FRONT)
				GetComponent<Image> ().sprite = _cardFace;
		}
    }

    public int cardValue
    {
        get { return _cardvalue; }
        set { _cardvalue = value; }
    }
	public CardState state
    {
        get { return _state; }
        set { _state = value; }
    }
    public bool initialized
    {
        get { return _initialized; }
        set { _initialized = value; }
    }
    public void falseCheck()
    {
        StartCoroutine(pause());
    }
    IEnumerator pause()
    {
		if (_state != CardState.SOLVED) {
			yield return new WaitForSeconds (1);
			if (_state == CardState.BACK)
				GetComponent<Image> ().sprite = _cardBack;
			else if (_state == CardState.FRONT)
				GetComponent<Image> ().sprite = _cardFace;
		}
			DO_NOT = false;
    }
}
