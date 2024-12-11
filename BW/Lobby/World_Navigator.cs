using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World_Navigator : NavigatorObserver
{
    [field: SerializeField] public override Navigator Navigator { get; set; }
    [field: SerializeField] public override Button Button { get; set; }
    [field: SerializeField] public override Image Image { get; set; }
    [field: SerializeField] public override ImageSprite Sprite { get; set; }

    private void Awake()
    {
        Button.onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        Navigator.Attach(this);
    }

    private void OnDisable()
    {
        Navigator.Detach(this);
    }

    public override void OnClick()
    {
        Navigator.NotifyObservers();
        Image.sprite = Sprite.onSprite;

        SceneLoadManager.instance.LoadScene("Stage_Outside");
    }

    public override void Notify(Navigator navigator)
    {
        Image.sprite = Sprite.offSprite;
    }
}