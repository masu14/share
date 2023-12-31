using System;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Merusenne.StageGimmick.LuminaGimmick
{
    /// <summary>
    /// GimmickObjectの発光ギミックを制御するスクリプトコンポーネント
    /// gimmickObjectにプレイヤーのショットを当てるとギミックが作動
    /// ショットの衝突時にgimmickObjectの孫オブジェクトにあるLuminaBoard,Barrierにショットの色を送信する
    /// </summary>
    public class GimmickController : MonoBehaviour
    {
        [SerializeField] private Light2D _point_light;                //子オブジェクトのGimmickLight

        private Color32 _blue = new Color32(127, 255, 255, 255);    //青色
        private Color32 _green = new Color32(56, 241, 104, 255);    //緑色
        private Color32 _red = new Color32(231, 69, 69, 255);       //赤色
        private Color32 _clear = Color.clear;                       //無色
        private float _time = 0f;

        //パラメータ
        [SerializeField] float _flash_speed = 2.0f;                 //点滅周期

        //UniRxのSubjectを定義
        private Subject<Color32> _collisionColor = new Subject<Color32>();
        private Subject<GameObject> _collisionObject = new Subject<GameObject>();

        //衝突したショットとその色を送信
        public IObservable<GameObject> OnCollisionObj => _collisionObject;
        public IObservable<Color32> OnCollision => _collisionColor;

        private void Update()
        {
            _time += Time.deltaTime;
            _point_light.intensity = Mathf.Sin(_time * _flash_speed);
        }

        //ショットと衝突したときにギミック作動
        private void OnCollisionEnter2D(Collision2D collision)
        {
            //タグを取得
            string shotTag = collision.gameObject.tag;

            //タグからショットの色を取得し、衝突したショットとその色を送信
            if (shotTag == "Shot_blue" || shotTag == "Shot_green" || shotTag == "Shot_red")
            {
                Color32 collisionColor = GetShotColor(shotTag);     //色を取得

                _point_light.color = collisionColor;                  //取得した色に発光
                _collisionColor.OnNext(collisionColor);             //色を送信
                _collisionObject.OnNext(collision.gameObject);      //衝突したショットを送信
            }

        }

        //タグから色を取得する
        private Color32 GetShotColor(string shotTag)
        {
            switch (shotTag)
            {
                case "Shot_blue":   //青色
                    return _blue;
                case "Shot_green":  //緑色
                    return _green;
                case "Shot_red":    //赤色
                    return _red;
                default:            //デフォルト値
                    return _clear;
            }
        }
    }

}
