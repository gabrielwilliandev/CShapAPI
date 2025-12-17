app.controller('AdminCtrl', function ($scope, $http, API_URL) {

    // Aba inicial
    $scope.activeTab = 'transactions';

    $scope.setActiveTab = function (tab) {
        $scope.activeTab = tab;

        if (tab === 'transactions') loadTransactions();
        if (tab === 'users') loadUsers();
        if (tab === 'categories') loadCategories();
    };

    // Dados
    $scope.globalTransactions = [];
    $scope.users = [];
    $scope.categories = [];
    $scope.newCategoryName = "";

    function loadTransactions() {
        $http.get(API_URL + '/transacao/admin/all')
            .then(res => $scope.globalTransactions = res.data);
    }

    function loadUsers() {
        $http.get(API_URL + '/user')
            .then(res => $scope.users = res.data);
    }

    function loadCategories() {
        $http.get(API_URL + '/category')
            .then(res => $scope.categories = res.data);
    }

    $scope.createCategory = function () {
        if (!$scope.newCategoryName) return;

        $http.post(API_URL + '/category', { name: $scope.newCategoryName })
            .then(() => {
                $scope.newCategoryName = "";
                loadCategories();
            });
    };

    $scope.deleteCategory = function (id) {
        if (!confirm("Deseja excluir?")) return;

        $http.delete(API_URL + '/category/' + id)
            .then(loadCategories);
    };

    // Inicialização
    loadTransactions();
    loadUsers();
    loadCategories();
});
