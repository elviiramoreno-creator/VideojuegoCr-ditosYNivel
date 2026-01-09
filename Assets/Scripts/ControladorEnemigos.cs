using System;
using UnityEngine;
public class ControladorEnemigos : MonoBehaviour
{
    public enum EstadoEnemigo
    //lista de estados del enemigo

    {
        Patrullando,
        Atacando,
        Muriendo,
    }
    //uso lista de estados
    public EstadoEnemigo estado;
    //variables
    public float velocidad;
    public Rigidbody2D rb;
    public GameObject graficos;
    public GameObject baston;
    public RaycastHit2D rayoSuelo;
    public RaycastHit2D rayoPared;






    public void Update()
    {
        switch (estado)
        {
            case EstadoEnemigo.Patrullando:
                //lógica de patrullaje
                Patrullar();
                break;
            case EstadoEnemigo.Atacando:
                //lógica de ataque
                Atacar();
                break;
            case EstadoEnemigo.Muriendo:
                //lógica de muerte
                Morir();
                break;
            default:
                Debug.Log("No se haya seleccionado estado alguno");
                break;

        }
    }
    public void Patrullar()
    {
        //comprobar si hay suelo delante
        rayoSuelo = Physics2D.Raycast(baston.transform.position, Vector2.down, 0.3f);
        //comprobar si hay pared delante
        //moverse
        if (rayoSuelo.collider == true)
        {
            rb.linearVelocity = new Vector2(velocidad, rb.linearVelocity.y);
        }
        else if (rayoSuelo.collider == false )
        {
            //girar
            gameObject.transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
            velocidad = -velocidad;
        } 
        
    }
    public void Atacar()
    {

    }
    //método destruir enemigo
    public void Morir()
    {
        Destroy(gameObject);
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawLine(baston.transform.position,
        baston.transform.position + Vector3.down*0.3f);
    }
}
