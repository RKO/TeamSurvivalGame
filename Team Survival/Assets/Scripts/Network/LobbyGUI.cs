using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections.Generic;

public class LobbyGUI : MonoBehaviour {
    private string[] NoNames = new string[] { "Nemo", "Nullface", "The Void", "Empty Space", "Vaccume", "Wasted Space" };

    private NetworkManager networkManager;
    private const int ListItemHeight = 31;
    private const int MatchListInterval = 5;
    private double matchListTimer = 0;
    private List<GameObject> listItems;

    public GameObject loadingPanel;
    public Image loadingSpinner;

    public Text textHostName;
    public RectTransform scrollContent;

    public GameObject listItemPrefab;

	// Use this for initialization
	void Start () {
        networkManager = NetworkManager.singleton;

        listItems = new List<GameObject>();

        //Make sure we refresh as soon as we can.
        matchListTimer = MatchListInterval;

        if (networkManager.matchMaker == null)
            networkManager.StartMatchMaker();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (networkManager.matchMaker != null && matchListTimer >= MatchListInterval) {
            networkManager.matchMaker.ListMatches(0, 20, "", OnMatchList);
            matchListTimer = 0;
        }

        if (loadingPanel.activeInHierarchy)
        {
            loadingSpinner.transform.Rotate(new Vector3(0, 0, -90 * Time.deltaTime));
        }

        matchListTimer += Time.deltaTime;
    }

    public void ActionStartHost() {
        string hostName = textHostName.text;

        if (hostName.Length == 0)
        {
            int i = Random.Range(0, NoNames.Length);
            hostName = NoNames[i] +" " + Random.Range(0, 10000);
        }

        networkManager.matchMaker.CreateMatch(hostName, networkManager.matchSize, true, "", networkManager.OnMatchCreate);
    }

    private void OnMatchList(ListMatchResponse response)
    {
        networkManager.OnMatchList(response);

        int largest = Mathf.Max(response.matches.Count, listItems.Count);

        int activeItems = 0;
        for (int i=0; i< largest; i++)
        {
            if (response.matches.Count > i)
            {
                MatchDesc item = response.matches[i];

                if (listItems.Count <= i)
                    listItems.Add(MakeListItem(i));

                SetListItemValues(item, i);
                activeItems++;
            }
            else if (listItems.Count > i) {
                listItems[i].SetActive(false);
            }
        }


        Vector2 sizeDelta = scrollContent.sizeDelta;
        sizeDelta.y = 31 * activeItems;
        scrollContent.sizeDelta = sizeDelta;
    }

    private GameObject MakeListItem(int position) {
        GameObject item = GameObject.Instantiate(listItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;

        item.transform.position = new Vector3(0, (position * ListItemHeight) * -1, 0);

        item.transform.SetParent(scrollContent, false);
        return item;
    }

    private void SetListItemValues(MatchDesc item, int itemNumber) {
        GameObject listItem = listItems[itemNumber];
        listItem.SetActive(true);

        listItem.GetComponentInChildren<Text>().text = item.name;
        listItem.GetComponentInChildren<Button>().onClick.RemoveAllListeners();

        listItem.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            JoinMatch(item);
        });
    }

    private void JoinMatch(MatchDesc match) {
        networkManager.matchMaker.JoinMatch(match.networkId, "", OnMatchJoined);
        loadingPanel.SetActive(true);
    }

    private void OnMatchJoined(JoinMatchResponse response)
    {
        //If it is a success, just keep it running.
        if (!response.success)
        {
            loadingPanel.SetActive(false);
            Debug.LogWarning("Can't join match. " + response.extendedInfo);
        }

        networkManager.OnMatchJoined(response);
    }

    public void ActionExitGame()
    {
        Application.Quit();
    }
}
