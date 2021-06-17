using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public UIDocument UIDocument;

    private GameObject playerPlane;
    private PlaneController planeControler;

    private Label speedLabel;
    private Label damageLabel;
    private Label pos;
    private Button menu;

    void Start()
    {
        var root = UIDocument.rootVisualElement;
        // get ui elements by name
        speedLabel = root.Q<Label>("Speed");
        damageLabel = root.Q<Label>("Damage");
        pos = root.Q<Label>("Position");
        menu = root.Q<Button>("Menu");

        menu.RegisterCallback<ClickEvent>(ev => SceneManager.LoadScene("Menu"));
      
        playerPlane = GameObject.Find("PlayerPlane");
        planeControler = playerPlane.GetComponent<PlaneController>();
    }


    private void Update()
    {
        int speed = (int)planeControler.rb.velocity.magnitude * 3;
        speedLabel.text = $"Speed: {speed}mph";

        int dmg = (planeControler.maxHitPoints-planeControler.hitPoints) * 100 / planeControler.maxHitPoints;
        damageLabel.text = $"Damage: {dmg}%";

        pos.text = $"(x,y,z)={planeControler.rb.position}";
    }

}