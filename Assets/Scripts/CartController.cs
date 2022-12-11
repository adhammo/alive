using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CartController : MonoBehaviour
{
        public GameObject BOOM;
        public GameObject CART;
        public GameObject DOOR;

        public void Function_BOOM()
        {
            BOOM.SetActive(true); //effect
            DOOR.SetActive(false); // destroy gate
            CART.SetActive(false);  //destroy car
        }


}
