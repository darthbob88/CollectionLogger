function AlbumViewModel(app, dataModel) {
    var self = this;

    var artistObject = function (data) {
        this.album = ko.observableArray(data.album);
        this.artist_ID = data.artist_ID;
        this.artist_name = data.artist_name;
        this.artist_link = "#albums/" + this.artist_ID;
        this.showDetails = ko.observable(false);
    };

    self.artists = ko.observableArray();

    self.getAlbumList = function (artistID) {
        if (artistID.album().length == 0) {
            dataModel.getArtistDetails(artistID.artist_ID).success(function (data) {
                artistID.album(data.album);
            });
        }
        artistID.showDetails(!artistID.showDetails());
    };

    Sammy(function () {
        this.get('#albums', function () {
            if (self.artists().length > 1) return; //Already got all the data we need.
            self.artists.removeAll();
            // Make a call to the protected Web API by passing in a Bearer Authorization Header
            $.ajax({
                method: 'get',
                url: app.dataModel.getArtistsUrl,
                contentType: "application/json; charset=utf-8",
                headers: {
                    'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
                },
                success: function (data) {
                    data.forEach(function (currentValue) {
                        self.artists.push(new artistObject(currentValue));
                    });
                }
            });
            app.view(self);  //this line is added
        });
        this.get("#albums/:artist_id", function () {
            //this.app.runRoute('get', '#albums');
            jQuery.getJSON(dataModel.getArtistsUrl + "/" + this.params["artist_id"]).success(function (data) {
                self.artists.removeAll();
                self.artists.push(new artistObject(data));
            });

        });
    });

    return self;
}

app.addViewModel({
    name: "Albums",
    bindingMemberName: "albums",
    factory: AlbumViewModel
});
