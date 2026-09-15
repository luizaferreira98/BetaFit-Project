using Microsoft.AspNetCore.Http;

namespace BetaFit.UI.Helpers;

/// <summary>Compatibiliza URLs legadas do seed com os assets públicos atuais.</summary>
public static class ProductMediaPath
{
    private const string Folder = "/images/products/";
    private const string Fallback = "betafit_progression_line_clean.png";

    private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Boné Beta Fit Branco.png"] = "betafit_bone_branco.png",
        ["Boné Beta Fit Grafite.png"] = "betafit_bone_grafite.png",
        ["Boné Beta Fit Preto.png"] = "betafit_bone_preto.png",
        ["Boné Beta Fit Verde Neon.png"] = "betafit_bone_verde_neon.png",
        ["Calça Fitness Feminina Modelo.png"] = "betafit_calca_feminina_grafite.png",
        ["Calça Jogger Essential Cinza.png"] = "betafit_calca_cinza_com_camiseta.png",
        ["Legging Cargo Neon.png"] = "betafit_calca_feminina_neon.png",
        ["Legging Performance Grafite.png"] = "betafit_calca_feminina_grafite.png",
        ["Moletom Canguru Beta Fit.png"] = "betafit_moletom_masculino.png",
        ["Regata Dry Performance Preta.png"] = "betafit_regata_preta_textura.png",
        ["Regata Dry Performance Cinza - Frente.png"] = "betafit_regata_cinza_frente_urso.png",
        ["Regata Dry Performance Cinza - Textura.png"] = "betafit_regata_cinza_textura.png",
        ["Camiseta Oversized Core Discipline.png"] = "betafit_core_discipline_clean.png",
        ["Camiseta Oversized Move Forward.png"] = "betafit_progression_line_clean.png",
        ["Camiseta Oversized Power Elements.png"] = "betafit_power_elements_robusto.png",
        ["Camiseta Asta - Frente.png"] = "Camisa Asta Frente.png",
        ["Camiseta Asta - Costas.png"] = "Camisa Asta Costa.png",
        ["Camiseta Nami - Frente.png"] = "Camisa Nami Frente.png",
        ["Camiseta Nami - Costas.png"] = "Camisa Nami.png",
        ["Camiseta Satoro Gojo - Frente.png"] = "Camisa Satoro Gojo Frente.png",
        ["Camiseta Satoro Gojo - Costas.png"] = "Camisa Satoro Gojo.png",
        ["Camiseta Zoro - Frente.png"] = "Camisa Zoro Frente.png",
        ["Camiseta Zoro - Costas.png"] = "Camisa Zoro Costa.png",
        ["Camiseta Fitness Premium Gemini 01.jpg"] = "Gemini_Generated_Image_66mtea66mtea66mt (1).jpg",
        ["Camiseta Fitness Premium Gemini 02.jpg"] = "Gemini_Generated_Image_p2umigp2umigp2um (1).jpg",
        ["Camiseta Fitness Dois Modelos.png"] = "composicao_dois_modelos_costas_c (1).png",
        ["Camiseta Fitness Feminina Separada.png"] = "mulher_separada_9x16.jpg - Crop (1).png",
        ["Camiseta Fitness Masculina Costas.png"] = "homem_costas_sem_protecao_9x16.j (1).png",
        ["Camiseta Fitness Masculina Sem Braços Cruzados.png"] = "homem_sem_bracos_cruzados_9x16.j (1).png",
        ["Camiseta Fitness Modelo Centro.png"] = "foto_meio_9x16_corpo.png",
        ["Camiseta Fitness Modelo Direita.png"] = "foto_direita_9x16_corpo.jpg - Cr (1).png",
        ["Camiseta Beta Fit Mercedes Masculina - Frente.png"] = "homem_frente_mercedes_betafit_9x (1).png",
        ["Camiseta Beta Fit Mercedes Masculina - Costas.png"] = "homem_costas_mercedes_betafit_mi (1).png",
        ["Camiseta Beta Fit Ferrari Masculina - Frente.png"] = "homem_ferrari_frente_academia_9x (1).png",
        ["Camiseta Beta Fit Ferrari Masculina - Costas.png"] = "homem_ferrari_costas_academia_9x (1).png",
        ["Camiseta Beta Fit Ferrari Feminina - Frente.png"] = "mulher_ferrari_frente_academia_9x (1).png",
        ["Camiseta Beta Fit Ferrari Feminina - Costas.png"] = "mulher_ferrari_costas_academia_9 (1).png",
        ["Camiseta Beta Fit Feminina Mais Treinada - Frente.png"] = "mulher_frente_9x16.jpg - Crop 9_ (1).png",
        ["Camiseta Beta Fit Feminina Mais Treinada - Costas.png"] = "mulher_costas_mais_treinada_v2_9 (1).png",
        ["Camiseta Beta Fit Feminina Treino.png"] = "mulher_frente_9x16.jpg - Crop 9_ (1).png",
        ["Garrafa Beta Fit Centro.png"] = "garrafa_centro_9x16.png",
        ["Garrafa Beta Fit Direita.png"] = "garrafa_direita_9x16.png",
        ["Garrafa Beta Fit Esquerda.png"] = "garrafa_esquerda_9x16.png",
        ["Garrafa Ferrari.png"] = "garrafa_ferrari_9x16.png",
        ["Garrafa Luffy.png"] = "garrafa_luffy_9x16.png",
        ["Garrafa Mercedes.png"] = "garrafa_mercedes_9x16.png",
        ["Garrafa Noelle.png"] = "garrafa_noelle_9x16.png",
        ["Garrafa Red Bull Racing.png"] = "garrafa_red_bull_racing_9x16.png",
        ["Garrafa Sanji.png"] = "garrafa_sanji_9x16.png",
        ["Garrafa Sem Texto com Alça.png"] = "garrafa_sem_texto_alca_02.png",
        ["Garrafa Yuno.png"] = "garrafa_yuno_9x16.png",
        ["Garrafa Zoro.png"] = "garrafa_zoro_9x16.png",
        ["Galão Beta Fit Grafite.png"] = "betafit_galao_grafite_urso_9x16.png",
        ["Galão Beta Fit Preto.png"] = "betafit_galao_preto_beta_9x16.png",
        ["Shaker Beta Fit 700ml.png"] = "shaker_betafit_9x16.png",
        ["Shaker Beta Fit Branco 700ml.png"] = "betafit_shaker_branco_urso_9x16.png",
        ["Shaker Beta Fit Preto 700ml.png"] = "betafit_shaker_preto_beta_9x16.png",
        ["Short Feminino Performance.png"] = "betafit_short_feminino_9x16.png.png",
        ["Short Feminino Performance Branco.png"] = "betafit_short_feminino_branco.png",
        ["Short Masculino Performance.png"] = "betafit_short_masculino_9x16.png",
        ["Short Masculino Performance Branco.png"] = "betafit_short_masculino_branco.png",
        // Não há fotos de tênis no pacote original. Usamos reservas distintas
        // para que os cinco produtos não pareçam a mesma foto duplicada.
        ["Tênis Classic White.png"] = "betafit_short_masculino_branco.png",
        ["Tênis Runner Flex Black.png"] = "betafit_short_masculino_9x16.png",
        ["Tênis Runner Panther Black.png"] = "betafit_power_elements_robusto.png",
        ["Tênis Runner Panther White.png"] = "betafit_calca_cinza_com_camiseta.png",
        ["Tênis Urban Low White.png"] = "betafit_moletom_masculino.png"
    };

    private static readonly HashSet<string> KnownFiles = Aliases.Values.ToHashSet(StringComparer.OrdinalIgnoreCase);

    public static PathString Normalize(PathString requestPath)
    {
        var path = requestPath.Value;
        if (string.IsNullOrWhiteSpace(path) || !path.StartsWith(Folder, StringComparison.OrdinalIgnoreCase)) return requestPath;
        var fileName = Uri.UnescapeDataString(path[Folder.Length..]);
        if (Aliases.TryGetValue(fileName, out var target)) return new PathString(Folder + target);
        return KnownFiles.Contains(fileName) ? requestPath : new PathString(Folder + Fallback);
    }
}
