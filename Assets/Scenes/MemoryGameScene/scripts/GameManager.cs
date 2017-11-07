using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Sprite[] cardFace;
    public Sprite cardBack;
    public GameObject[] cards;
    public Text matchText;

    public bool _init = false;
    public int matches = 6;

    // Update is called once per frame 
    void Update()
    {
        if (!_init)
            initializeCards();
		if (Input.GetMouseButtonUp(0) && !Card.DO_NOT)
            checkCards();
    }
    void initializeCards()
    {
        for (int id = 0; id < 2; id++)
        {
            for (int i = 1; i < 7; i++)
            {
                bool test = false;
                int choise = 0;
                while (!test)
                {
                    choise = Random.Range(0, cards.Length);
                    test = !(cards[choise].GetComponent<Card>().initialized);
                }
                cards[choise].GetComponent<Card>().cardValue = i;
                cards[choise].GetComponent<Card>().initialized = true;
            }
        }
        foreach (GameObject c in cards)
            c.GetComponent<Card>().setupGraphics();

        if (!_init)
            _init = true;

    }
    public Sprite getCardBack()
    {
        return cardBack;
    }
    public Sprite getCardFace(int i)
    {
        return cardFace[i - 1];
    }
    void checkCards()
    {
		Card.DO_NOT = true;
        List<int> c = new List<int>();

        for (int i = 0; i < cards.Length; i++)
        {
			if (cards[i].GetComponent<Card>().state == CardState.FRONT)
                c.Add(i);
        }
		if (c.Count == 2)
			cardComparison (c);
		else
			Card.DO_NOT = false;
    }
    void cardComparison(List<int> c)
    {
		Card.DO_NOT = true;

		CardState x = CardState.BACK;

        if (cards[c[0]].GetComponent<Card>().cardValue == cards[c[1]].GetComponent<Card>().cardValue)
        {
			x = CardState.SOLVED;
            matches--;
            matchText.text = "Number of Pairs:" + matches;
            if (matches == 0)
                SceneManager.LoadScene(8);
        }
        for (int i = 0; i < c.Count; i++)
        {
            cards[c[i]].GetComponent<Card>().state = x;
            cards[c[i]].GetComponent<Card>().falseCheck();
        }
    }
}