using UnityEngine.Networking;


public class Player : NetworkBehaviour {
    public int PlayerID { get; private set; }

    public void Initialize(int id) {
        PlayerID = id;
    }
}
