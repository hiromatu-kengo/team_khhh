using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Vector4 quat;
    public Vector3 Input;
    public Vector3 Output;
   
    // Update is called once per frame
    void Update()
    {
        // 入力されたオイラーをQに変換
        var rot = Quaternion.Euler(Input);
　　　  // Qをオイラーに変換
        Output = rot.eulerAngles;
        // Qをトランスフォームに代入
        transform.rotation = rot;
        // quat
        quat = new Vector4(rot.x, rot.y, rot.z, rot.w);
    }
}
