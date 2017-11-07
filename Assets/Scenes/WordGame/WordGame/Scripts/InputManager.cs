using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Word : MonoBehaviour
{
    public String word;
    private WordCells wordCells;
    private WordLetters wordLetters;

    public void Init(String word)
    {
        this.word = word;
        wordCells = gameObject.AddComponent<WordCells>() as WordCells;
        wordCells.Init(word);
        wordLetters = gameObject.AddComponent<WordLetters>() as WordLetters;
        wordLetters.Init(word);
    }

    public bool CheckWord()
    {
        String currentWord = wordCells.CurrentWord();
        if (currentWord.Length == word.Length)
        {
            AudioClip sound = Resources.Load<AudioClip>((currentWord.Equals(word)) ? "correct" : "wrong");
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(sound);

            if (currentWord.Equals(word)) return true;
        }
        return false;
    }

    public void Destroy()
    {
        wordCells.Destroy();
        wordLetters.Destroy();
    }

    private class WordCells : MonoBehaviour
    {
        private String word;
        private GameObject[] cells;

        public void Init(String word)
        {
            this.word = word;

            int count = word.Length;
            float width = (count * 1.3f);
            cells = new GameObject[count];
            for (int i = 0; i < count; i ++) {
                GameObject cell = (GameObject)Instantiate(Resources.Load("Cell"));
                cell.transform.position = new Vector3(width / 2.0f - i * 1.3f, 0.5f, 0.0f);
                cell.GetComponent<SpriteRenderer>().sortingOrder = 0;
                cell.tag = "Cell";
                cells[i] = cell;
            }
        }

        public String CurrentWord()
        {
            String str = "";
            foreach (GameObject cell in cells)
            {
                if (cell.transform.childCount > 0) {
                    str = cell.transform.GetChild(0).GetComponent<Tile>().letter + str;
                }
            }
            return str;
        }

        public void Destroy()
        {
            foreach (GameObject cell in cells)
            {
                Destroy(cell);
            }
        }
    }

    private class WordLetters : MonoBehaviour
    {
        private String word;
        private GameObject[] letters;

        public void Init(String word)
        {
            this.word = word;

            // Shuffle string
            List<char> shuffle = new List<char>();
            shuffle.AddRange(word);
            for (int i = 0; i < shuffle.Count; i++) {
                char temp = shuffle[i];
                int randomIndex = UnityEngine.Random.Range(i, shuffle.Count);
                shuffle[i] = shuffle[randomIndex];
                shuffle[randomIndex] = temp;
            }

            int count = word.Length;
            float width = (count * 1.3f) + ((count - 1.0f) * 0.2f);
            letters = new GameObject[count];
            for (int i = 0; i < count; i ++) {
                GameObject letter = (GameObject)Instantiate(Resources.Load("Tile"));
                letter.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/letter_" + shuffle[i]);
                letter.transform.position = new Vector3(width / 2.0f - i * 1.3f - 0.2f * i, -1.5f, 0.0f);
                letter.GetComponent<SpriteRenderer>().sortingOrder = 1;
                letter.tag = "Tile";
                letter.GetComponent<Tile>().letter = shuffle[i];
                letters[i] = letter;
            }
        }

        public void Destroy()
        {
            foreach (GameObject letter in letters)
            {
                Destroy(letter);
            }
        }
    }
}

public class WordFactory
{
    public enum Difficulty { EASY,  MEDIUM, HARD };

    private String[] easyWords = { "RED", "BLUE", "GREEN", "DOG", "CAT", "FISH", "BIRD" };
    private String[] mediumWords = { "YELLOW", "PURPLE", "ORANGE", "RABBIT", "BEAVER", "GIRAFFE" };
    private String[] hardWords = { "EXTREMELY", "COMPLICATED", "COMPOSITION", "ENORMOUS", "BUSINESS", "CROCODILE" };

    private GameObject gameObject;

    public WordFactory(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

    public Word CreateWord(Difficulty difficulty)
    {
        String str = null;
        switch(difficulty)
        {
            case Difficulty.EASY:
                str = easyWords[UnityEngine.Random.Range(0, easyWords.Length - 1)];
                break;
            case Difficulty.MEDIUM:
                str = mediumWords[UnityEngine.Random.Range(0, mediumWords.Length - 1)];
                break;
            case Difficulty.HARD:
                str = hardWords[UnityEngine.Random.Range(0, hardWords.Length - 1)];
                break;
        }

        return CreateWord(str);
    }

    public Word CreateWord(String str)
    {
        str = str.ToUpper();
        Word word = gameObject.AddComponent<Word>() as Word;
        word.Init(str);
        return word;
    }
}

public class InputManager : MonoBehaviour
{
    private bool dragging = false;
    private GameObject draggedObject;
    private Vector2 touchOffset;
    private Word word;

    private void Start()
    {
        WordFactory wordFactory = new WordFactory(gameObject);
        // word = wordFactory.CreateWord("FUCK");
        word = wordFactory.CreateWord(WordFactory.Difficulty.MEDIUM);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            DragOrPickUp();
        }
        else if (dragging)
        {
            DropItem();
        }
    }

    Vector2 CurrentTouchPosition
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void DragOrPickUp()
    {
        var inputPosition = CurrentTouchPosition;
        if (dragging)
        {
            draggedObject.transform.position = inputPosition + touchOffset;
        }
        else
        {
            var layerMask = 1 << 0;
            RaycastHit2D[] touches = Physics2D.RaycastAll(inputPosition, inputPosition, 0.5f, layerMask);
            if (touches.Length > 0)
            {
                foreach (var hit in touches)
                {
                    if (hit.transform != null && hit.transform.tag == "Tile")
                    {
                        dragging = true;
                        draggedObject = hit.transform.gameObject;
                        touchOffset = (Vector2)hit.transform.position - inputPosition;
                        hit.transform.GetComponent<Tile>().PickUp();
                    }
                }
            }
        }
    }

    void DropItem()
    {
        dragging = false;
        draggedObject.transform.localScale = new Vector3(1, 1, 1);
        draggedObject.GetComponent<Tile>().Drop();


        if (word.CheckWord())
        {
            StartCoroutine(NewWord());
        }
    }

    IEnumerator NewWord()
    {
        yield return new WaitForSeconds(2);
        word.Destroy();
        WordFactory wordFactory = new WordFactory(gameObject);
        word = wordFactory.CreateWord(WordFactory.Difficulty.MEDIUM);
    }
}
