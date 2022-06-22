using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MW
{
    public class Gear : MonoBehaviour
    {
        /// <summary>モジュール</summary>
        [Header("歯車の大きさ")]
        [SerializeField]
        private float _m;

        /// <summary>歯数</summary>
        [Header("歯数")]
        [SerializeField]
        private int _z;

        /// <summary>圧力角</summary>
        private float _a = 20;

        /// <summary>ピッチ</summary>
        private float _p => _m * Mathf.PI;

        /// <summary>歯たけ</summary>
        private float _h => _m * 2.25f;
        /// <summary>歯末</summary>
        private float _ha => _m * 1f;
        /// <summary>歯元</summary>
        private float _hf => _m * 1.25f;

        /// <summary>歯厚</summary>
        private float _s => _p * 0.5f;

        // http://2c-laboratory.sakura.ne.jp/DW/Q/Toothwidth.php?w=1&c=3
        /// <summary>歯幅</summary>
        private float _b => _m * 6;

        /// <summary>基準円直径</summary>
        private float _d => _z * _m;
        /// <summary>歯先円直径</summary>
        private float _da => _d + _m * 2;
        /// <summary>歯底円直径</summary>
        private float _df => _d - _m * 2.5f;

        private const string _teethName = "Teeth";

        private float _outerRadius => _da * 0.5f;
        private float _innerRadius => _df * 0.5f;

        private float angle = 0;

        void OnValidate()
        {
            EditorApplication.delayCall += () => Setting();
        }

        private void Setting()
        {
            // 中心部分の大きさ設定
            var xz = _innerRadius * 2;
            var y = _b * 0.5f;
            transform.localScale = new Vector3(xz, y, xz);

            // 子をすべて削除
            var childList = new List<GameObject>();
            foreach (Transform child in transform)
            {
                if (child.name == _teethName)
                {
                    childList.Add(child.gameObject);
                }
            }
            foreach (var child in childList)
            {
                DestroyImmediate(child);
            }

            // 歯を生成
            for (int i = 0; i < _z; i++)
            {
                var center = transform.position;

                var angle = 360 / _z * i;

                var rad = angle * Mathf.Deg2Rad;
                var r = _innerRadius + _h * 0.5f;
                var posX = center.x + r * Mathf.Sin(rad);
                var posZ = center.z + r * Mathf.Cos(rad);

                var teethObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                teethObj.name = _teethName;
                teethObj.transform.position = new Vector3(posX, center.y, posZ);
                teethObj.transform.LookAt(center);
                teethObj.transform.localScale = new Vector3(_s, _b, _h);
                teethObj.transform.SetParent(transform);
            }

            // すべてのコライダーに最大摩擦のマテリアルを設定
            var colliders = new List<Collider>();
            colliders.AddRange(GetComponents<Collider>());
            colliders.AddRange(GetComponentsInChildren<Collider>());

            var maxFriction = new PhysicMaterial();
            maxFriction.dynamicFriction = 1;
            maxFriction.staticFriction = 1;
            maxFriction.frictionCombine = PhysicMaterialCombine.Maximum;

            foreach (var collider in colliders)
            {
                collider.material = maxFriction;
            }
        }
    }
}
