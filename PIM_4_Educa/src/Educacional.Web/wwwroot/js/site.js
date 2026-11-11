// Acessibilidade: tamanho do texto e alto contraste, lembrados entre as visitas (localStorage).
(function () {
    var raiz = document.documentElement;
    var fonte = 100;

    function ler(chave) {
        try { return localStorage.getItem(chave); } catch (e) { return null; }
    }
    function salvar(chave, valor) {
        try { localStorage.setItem(chave, valor); } catch (e) { }
    }

    fonte = parseInt(ler('pim-fonte') || '100', 10) || 100;

    function aplicarFonte() {
        raiz.style.setProperty('--fonte', fonte + '%');
        salvar('pim-fonte', String(fonte));
    }

    var botaoMais = document.getElementById('btn-fonte-mais');
    var botaoMenos = document.getElementById('btn-fonte-menos');
    var botaoContraste = document.getElementById('btn-contraste');

    if (botaoMais) botaoMais.addEventListener('click', function () {
        fonte = Math.min(160, fonte + 10);
        aplicarFonte();
    });

    if (botaoMenos) botaoMenos.addEventListener('click', function () {
        fonte = Math.max(80, fonte - 10);
        aplicarFonte();
    });

    function atualizarBotaoContraste() {
        var ativo = raiz.getAttribute('data-tema') === 'contraste';
        if (botaoContraste) botaoContraste.setAttribute('aria-pressed', ativo ? 'true' : 'false');
    }

    if (botaoContraste) botaoContraste.addEventListener('click', function () {
        if (raiz.getAttribute('data-tema') === 'contraste') {
            raiz.removeAttribute('data-tema');
            salvar('pim-tema', '');
        } else {
            raiz.setAttribute('data-tema', 'contraste');
            salvar('pim-tema', 'contraste');
        }
        atualizarBotaoContraste();
    });

    atualizarBotaoContraste();
})();
