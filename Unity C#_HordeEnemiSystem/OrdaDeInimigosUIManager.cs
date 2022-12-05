using UnityEngine;
using TMPro;

public class OrdaDeInimigosUIManager : MonoBehaviour
{
    [SerializeField] GameObject OrdaUIGameObject;
    [Tooltip("Considere deixar a escala X da imagem em 100")]
    [SerializeField] Transform imageProgressoAtual;
    [Tooltip("Lista de textos presente na UI. Exemplos (texto de derrota, ou mini tutorial)")]
    [SerializeField] private TMP_Text[] textosDeInformacao;
    
    private void Start() 
    {
        AtivarUI(false);
        imageProgressoAtual.localScale = new Vector3(100 , imageProgressoAtual.localScale.y, imageProgressoAtual.localScale.z);
    }

    /// <summary>
    /// Atualiza a barra de estutus da UI com base na quantia maxima e atual de inimigos.
    /// </summary>
    public void AtualizarProgressoDaOrda(int quantidadeMaxInimigo, int quantidadeAtual)
    {
        if(quantidadeMaxInimigo < 0)
        {
            throw new System.Exception("quantidadeMaxInimigo não pode ser menor que zero");
        }

        if(quantidadeAtual < 0)
        {
            quantidadeAtual = 1;
        }

        float progresso = (quantidadeAtual * 100 ) / quantidadeMaxInimigo;
        imageProgressoAtual.localScale = new Vector3(progresso , imageProgressoAtual.localScale.y, imageProgressoAtual.localScale.z);
    }

    /// <summary>
    /// Ativar ou desativa a UI com a barra de progresso
    /// </summary>
    public void AtivarUI(bool ativar)
    {
        OrdaUIGameObject.SetActive(ativar);
    }

    /// <summary>
    /// Atribui um texto na UI referente em uma lista de textos
    /// </summary>
    public void SetTextoIU(int index, string mensagem)
    {
        textosDeInformacao[index].text = mensagem;
    }

    /// <summary>
    /// Limpa um texto na UI em uma posição especifica
    /// </summary>
    public void LimparTextoUI(int index)
    {
        if(index < textosDeInformacao.Length || index > 0)
        {
            textosDeInformacao[index].text = string.Empty;
        }
    }

    /// <summary>
    /// Limpa todos os textos na UI
    /// </summary>
    public void LimparTodosTextoUI()
    {
        for(int i = 0; i < textosDeInformacao.Length; i++)
        {
            textosDeInformacao[i].text = string.Empty;
        }
    }
}
