using System.Collections;
using OldScripts; //onde está InimigoController
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OrdaDeInimigosController : MonoBehaviour
{
    [Header("Inimigos")]
    [SerializeField] GameObject[] chefao;
    [SerializeField] GameObject[] inimigos;
    [Space]
    [Header("Spawn config")]
    [SerializeField] Transform[] locaisDeSpawn;
    [SerializeField] int quantidadeDeOrdas = 3;
    [SerializeField] int quantidadeInimigosPorOrda = 3;
    [SerializeField] int quantidadeChefoes = 2;
    [Space]
    [Header("Configs gerais")]
    [SerializeField] string tagPlayer = "Player";
    [Header("Mensagens")]
    [SerializeField] string textoInicial = "Elimite todos os inimigos";
    [SerializeField] string textoDeVitoria = "Venceu!";
    [SerializeField] string textoDeDerrota = "Falhou!";
    [SerializeField][UnityEngine.TextArea] string textoMotivoDaDerrota = "Você abandou a orda!";
    [SerializeField][UnityEngine.TextArea] string textoMiniTutorial = "-Mate todos os inimigos. \n-Não abandone a orda."; 

    private OrdaDeInimigosUIManager ordaManagerUI;
    private BoxCollider Trigger;

    private int maxInimigos = 0; 
    private int inimigosMortos = 0;
    private int ordaAtual = 0;
    private int quantidadeInimigosAtual = 0;
    private bool spawnarChefao = true;

    private void Awake() 
    {
        ordaManagerUI = FindObjectOfType<OrdaDeInimigosUIManager>();
        Trigger       = GetComponent<BoxCollider>();

        Trigger.isTrigger = true;

        for(int i = 0; i < quantidadeDeOrdas; i++)
        {
            maxInimigos += quantidadeInimigosPorOrda;
        }

        quantidadeInimigosAtual = maxInimigos;
    }

    //-------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag(tagPlayer))
        {
            AtivarOrdaDeInimigos();
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag(tagPlayer))
        {
            ordaManagerUI.AtivarUI(false);
            spawnarChefao = false;

            if(quantidadeInimigosAtual > 0)
            {
                Perdeu();
            }
        }
    }

    //-------------------------------------------------------------------------

    private void AtivarOrdaDeInimigos()
    {
        quantidadeInimigosAtual = maxInimigos;

        StartCoroutine(Spawner(inimigos, locaisDeSpawn, quantidadeInimigosPorOrda));
        ordaManagerUI.AtivarUI(true);
        ordaManagerUI.AtualizarProgressoDaOrda(maxInimigos, quantidadeInimigosAtual);
        StartCoroutine(AtualizarTextoUI(textoInicial, 0, 5));   
        StartCoroutine(AtualizarTextoUI(textoMiniTutorial, 1, 8));   
    }

    private IEnumerator Spawner(GameObject[] inimigos, Transform[] locaisDeSpawn, int quantidadeDeInimigos)
    {
        yield return new WaitForSeconds(2);

        ordaAtual++;

        for(int i = 0; i < quantidadeDeInimigos; i++)
        {
            int posAleatorio  = Random.Range(0, locaisDeSpawn.Length);
            int inimAleatorio = Random.Range(0, inimigos.Length);

            //Trocar por um Pool de objetos
            Instantiate(inimigos[inimAleatorio], locaisDeSpawn[posAleatorio].position, Quaternion.identity).
                GetComponent<InimigoController>().OnDie += InimigoMorto;
        }
    }

    //-------------------------------------------------------------------------

    private void InimigoMorto(InimigoController inimigo)
    {
        inimigo.GetComponent<InimigoController>().OnDie -= InimigoMorto;

        quantidadeInimigosAtual--;

        if(ordaAtual < quantidadeDeOrdas)
            inimigosMortos++;

        if(inimigosMortos == quantidadeInimigosPorOrda && ordaAtual <= quantidadeDeOrdas)
        {
            StartCoroutine(Spawner(inimigos, locaisDeSpawn, quantidadeInimigosPorOrda));
            inimigosMortos = 0;
        }

        if(quantidadeInimigosAtual <= 0)
        {
            SpawnarChefe();
        }

        if(quantidadeInimigosAtual < 0)
        {
            ordaManagerUI.AtivarUI(false);
            Venceu();
        }

        ordaManagerUI.AtualizarProgressoDaOrda(maxInimigos, quantidadeInimigosAtual);
    }

    //-------------------------------------------------------------------------

    private IEnumerator AtualizarTextoUI(string text, int pos, int temp)
    {
        ordaManagerUI.SetTextoIU(pos, text);
        yield return new WaitForSeconds(temp);
        ordaManagerUI.LimparTextoUI(pos);
    }

    //-------------------------------------------------------------------------
    private void SpawnarChefe()
    {
        if(spawnarChefao)
        {
            StartCoroutine(AtualizarTextoUI("Derrote o Boss!", 0, 5));
            StartCoroutine(Spawner(chefao, locaisDeSpawn, quantidadeChefoes));
        }

        spawnarChefao = false;
    }

    //-------------------------------------------------------------------------

    private void Venceu()
    {
        maxInimigos = 0;
        quantidadeInimigosAtual = 0;
        inimigosMortos = 0;

        StartCoroutine(AtualizarTextoUI(textoDeVitoria, 0, 5));
        Destroy(this.gameObject, 10);
    }

    //-------------------------------------------------------------------------

    private void Perdeu()
    {
        quantidadeInimigosAtual = maxInimigos;
        inimigosMortos = 0;
        ordaAtual = 0;

        StartCoroutine(AtualizarTextoUI(textoMotivoDaDerrota, 2, 5));
        StartCoroutine(AtualizarTextoUI(textoDeDerrota, 0, 5));
        ordaManagerUI.LimparTextoUI(1);

        GameObject[] inimigos = GameObject.FindGameObjectsWithTag("Inimigo");

        for(int i = 0; i < inimigos.Length; i++)
        {
            inimigos[i].GetComponent<InimigoController>().OnDie -= InimigoMorto;
        }
    }
}