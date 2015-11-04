function MovieViewModel(app, dataModel) {
    var self = this;

    self.movies = ko.observableArray();

    Sammy(function () {
        this.get('#movies', function () {
            // Make a call to the protected Web API by passing in a Bearer Authorization Header
            $.ajax({
                method: 'get',
                url: dataModel.getMoviesUrl,
                contentType: "application/json; charset=utf-8",
                headers: {
                    'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
                },
                success: function (data) {
                    data.forEach(function (currentValue) {
                        self.movies.push(currentValue);
                    });
                }
            });
            app.view(self);  //this line is added
        });
    });

    return self;
}

app.addViewModel({
    name: "Movies",
    bindingMemberName: "movies",
    factory: MovieViewModel
});
