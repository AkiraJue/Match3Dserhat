using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputController : Singleton<InputController>
{
    RaycastHit hit;
    public List<GameObject> objectsOnStack = new List<GameObject>(); // Object clicked and start moving to stack.
    public List<GameObject> arrivedObject = new List<GameObject>(); // object that arrived stack. Then ready for check match.
    private void Awake()
    {
        Input.multiTouchEnabled = false;
    }
    private void Update()
    {
        if (GameManager.Instance.GameIsPlaying()) // If game is playing get input from user touch.
        {
            CheckForUserInput();
        }
    }

    private void CheckForUserInput() // Send ray to select object when user finger leave screen.
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (ReadyToCollect(hit.collider.gameObject))
                {
                    AddObjectOnStack(hit.collider.gameObject);
                }
            }
        }
    }
    private bool ReadyToCollect(GameObject obj) // Is this item ready for stack.
    {
        return obj != null && obj.tag.Contains("Collectable") && !objectsOnStack.Contains(obj);
    }

    public void AddObjectOnStack(GameObject obj) // Add selected object to our stack.
    {
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.GetComponent<Collider>().enabled = false;
        objectsOnStack.Add(obj);
        ReorderObjectList(obj);
    }
    private void ReorderObjectList(GameObject obj) // Reorder the list by type of object.
    {
        objectsOnStack = objectsOnStack.GroupBy(x => x.name).SelectMany(g => g).ToList();
        RepositionObjects(obj);
    }
    private void RepositionObjects(GameObject obj) // Change the position of the objects according to the final state of the list.
    {
        for (int i = 0; i < objectsOnStack.Count; i++)
        {
            Vector3 oldPosition = objectsOnStack[i].transform.position;
            Vector3 newPosition = new Vector3(-1 + (i * 0.333f), 0.3f, -1.62f);
            if (oldPosition != newPosition)
            {
                objectsOnStack[i].transform.DORotate(Quaternion.identity * Vector3.one, .2f);
                objectsOnStack[i].transform.DOMove(newPosition, .2f).SetEase(Ease.Linear) // Some animations while moving.
                    .OnComplete(() =>
                    {
                        if (obj != null && !arrivedObject.Contains(obj) && GameManager.Instance.GameIsPlaying())
                        {
                            arrivedObject.Add(obj);
                            CheckMatches();
                            BoosterManager.Instance.CheckHintInteractibility();
                        }
                    });
            }
        }
    }

    private void CheckMatches() // Check if there is a match on the list. If available, give priority to matching on the left.
    {
        List<IGrouping<string, GameObject>> matchList = arrivedObject.GroupBy(x => x.name).Where(g => g.Count() > 2).ToList();
        if (matchList.Count() > 0)
        {
            List<GameObject> selectedList = matchList.FirstOrDefault().OrderBy(p => p.transform.position.x).ToList();
            selectedList[0].transform.DOMove(selectedList[1].transform.position, 0.2f).SetEase(Ease.InBack); // Some animations while matching.
            selectedList[2].transform.DOMove(selectedList[1].transform.position, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    if (GameManager.Instance.GameIsPlaying())
                    {
                        arrivedObject.Remove(selectedList[i]);
                        objectsOnStack.Remove(selectedList[i]);
                        LevelGenerator.Instance.objectsOnScene.Remove(selectedList[i]);
                        Destroy(selectedList[i]);
                    }
                }
                ReorderObjectList(null);
                CheckOutOfSpace();
            });
        }
        else if(BoosterManager.Instance.hintButton.interactable) // There is no match but hint button used so there will be a guaranteed match on next move.
        {
            CheckOutOfSpace();
        }
    }
    private void CheckOutOfSpace() // If there is no space on Stack, then the game over.
    {
        if (objectsOnStack.Count >= 7)
        {
            UIManager.Instance.ShowOutOfBoxesPopUp();
        }
    }
}
