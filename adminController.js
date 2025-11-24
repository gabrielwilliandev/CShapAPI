app.controller('AdminCtrl', function ($scope, $http) {
    // Aba inicial
    $scope.activeTab = 'categorias';

    // Dados
    $scope.categorias = [];
    $scope.usuarios = [];
    $scope.todasTransacoes = [];
    $scope.novaCat = {};

    // --- FUNÇÕES DE CATEGORIA ---
    $scope.loadCategorias = function () {
        $http.get(API_URL + '/category').then(function (res) {
            $scope.categorias = res.data;
        });
    };

    $scope.criarCategoria = function () {
        $http.post(API_URL + '/category', $scope.novaCat)
            .then(function (res) {
                alert("Categoria criada!");
                $scope.novaCat = {};
                $scope.loadCategorias();
            }, function (err) {
                alert("Erro ao criar: " + JSON.stringify(err.data));
            });
    };

    $scope.deletarCategoria = function (id) {
        if (confirm("Tem certeza que deseja excluir esta categoria?")) {
            $http.delete(API_URL + '/category/' + id)
                .then(function () {
                    $scope.loadCategorias();
                }, function (err) {
                    alert("Erro: Categoria pode estar em uso.");
                });
        }
    };

    $scope.atualizarCategoria = function (cat) {
        $http.put(API_URL + '/category/' + cat.id, { Name: cat.name })
            .then(function () {
                cat.editing = false;
                alert("Atualizado!");
            }, function (err) {
                alert("Erro ao atualizar.");
            });
    };

    // --- FUNÇÕES DE USUÁRIO ---
    $scope.loadUsuarios = function () {
        $http.get(API_URL + '/user').then(function (res) {
            $scope.usuarios = res.data;
        });
    };

    // --- FUNÇÕES DE TRANSAÇÕES GLOBAIS ---
    $scope.loadTodasTransacoes = function () {
        // Chama o endpoint novo que criamos no Passo 1
        $http.get(API_URL + '/transacao/admin/all').then(function (res) {
            $scope.todasTransacoes = res.data;
        }, function (err) {
            console.error(err);
        });
    };

    // Inicialização: Carrega tudo ao abrir a página
    $scope.loadCategorias();
    $scope.loadUsuarios();
    $scope.loadTodasTransacoes();
});