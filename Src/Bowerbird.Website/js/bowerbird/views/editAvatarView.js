﻿
window.Bowerbird.Views.EditAvatarView = Backbone.View.extend({
    id: 'avatar-fieldset',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this,
        '_initMediaUploader',
        '_onUploadDone',
        '_onSubmitUpload',
        '_onUploadAdd'
        );
        this.mediaResourceItemViews = [];
        this.observation = options.observation;
        this.currentUploadKey = 0;
    },

    render: function () {
        this._initMediaUploader();
        return this;
    },

    _initMediaUploader: function () {
        $('#fileupload').fileupload({
            dataType: 'json',
            paramName: 'file',
            url: '/members/mediaresource/avatarupload',
            add: this._onUploadAd,
            submit: this._onSubmitUpload,
            done: this._onUploadDone
        });
    },

    _onUploadAdd: function (e, data) {
        var self = this;
        $.each(data.files, function (index, file) {
            if (file != null) {
                self.currentUploadKey++;
                var mediaResource = new Bowerbird.Models.MediaResource({ key: self.currentUploadKey });
                self.observation.addMediaResources.add(mediaResource);
                var mediaResourceItemView = new Bowerbird.Views.MediaResourceItemView({ mediaResource: mediaResource });
                self.mediaResourceItemViews.push(mediaResourceItemView);
                $('#media-resource-add-pane').before(mediaResourceItemView.render().el);
                loadImage(
                    data.files[0],
                    function (img) {
                        if (img instanceof HTMLImageElement) { // FF seems to fire this handler twice, on second time returning error, which we ignore :(
                            mediaResourceItemView.showTempMedia(img);
                            $('#media-resource-items').animate({ scrollLeft: 100000 });
                        }
                    },
                    {
                        maxHeight: 220
                    }
                );
            }
        });
        data.submit();
    },

    _onSubmitUpload: function (e, data) {
        data.formData = { key: this.currentUploadKey, originalFileName: data.files[0].name };
    },

    _onUploadDone: function (e, data) {
//        var mediaResource = _.find(this.observation.allMediaResources(), function (item) {
//            return item.get('key') == data.result.key;
//        });
        mediaResource.set(data.result);
        //$('#media-resource-items').animate({ scrollLeft: 100000 });
    }
});