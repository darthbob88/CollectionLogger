function AppDataModel() {
    var self = this;
    // Routes
    self.userInfoUrl = "/api/Me";
    self.siteUrl = "/";
    self.getAlbumsUrl = "api/albums";
    self.getArtistsUrl = "api/artists";
    self.getMoviesUrl = "api/movies";
    // Route operations

    // Other private operations

    // Operations

    // Data
    self.returnUrl = self.siteUrl;

    // Data access operations
    self.setAccessToken = function (accessToken) {
        sessionStorage.setItem("accessToken", accessToken);
    };

    self.getAccessToken = function () {
        return sessionStorage.getItem("accessToken");
    };
    self.getAlbums = function () {
        return jQuery.getJSON(self.getAlbumsUrl);
    };
    self.getArtists = function () {
        return jQuery.getJSON(self.getArtistsUrl);
    }; self.getArtistDetails = function (id) {
        return jQuery.getJSON(self.getArtistsUrl + "/" + id);
    };
    self.getMovies = function () {
        return jQuery.getJSON(self.getMoviesUrl);
    };
}
