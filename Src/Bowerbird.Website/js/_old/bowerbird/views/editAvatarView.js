﻿
window.Bowerbird.Views.EditAvatarView = Backbone.View.extend({
    id: 'avatar-fieldset',

    events: {
        'click .remove-media-resource-button': 'removeMediaResource'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this,
        'render',
        '_initMediaUploader',
        '_onUploadDone',
        '_onSubmitUpload',
        '_onUploadAdd',
        'removeMediaResource'
        );
        this.group = options.group;
        this.currentUploadKey = 0;
        this.avatarItemView = null;
    },

    render: function () {
        ich.AvatarChooseFile().appendTo($('#avatar-add-pane'));
        this._initMediaUploader();
        return this;
    },

    _initMediaUploader: function () {
        $('#fileupload').fileupload({
            dataType: 'json',
            paramName: 'file',
            url: '/mediaresource/avatarupload',
            add: this._onUploadAd,
            submit: this._onSubmitUpload,
            done: this._onUploadDone,
            limitConcurrentUploads: 1
        });
    },

    _onUploadAdd: function (e, data) {
        //        var self = this;
        //        $.each(data.files, function (index, file) {
        //            if (file != null) {
        //                self.currentUploadKey++;
        //                var mediaResource = new Bowerbird.Models.MediaResource({ key: self.currentUploadKey });
        //                self.group.set('avatar', mediaResource);
        //                //                self.observation.addMediaResources.add(mediaResource);
        //                //                var mediaResourceItemView = new Bowerbird.Views.MediaResourceItemView({ mediaResource: mediaResource });
        //                //                self.mediaResourceItemViews.push(mediaResourceItemView);
        //                $('#media-resource-add-pane').before(mediaResourceItemView.render().el);
        //                loadImage(
        //                    data.files[0],
        //                    function (img) {
        //                        if (img instanceof HTMLImageElement) { // FF seems to fire this handler twice, on second time returning error, which we ignore :(
        //                            mediaResourceItemView.showTempMedia(img);
        //                            $('#media-resource-items').animate({ scrollLeft: 100000 });
        //                        }
        //                    },
        //                    {
        //                        maxHeight: 220
        //                    }
        //                );
        //            }
        //        });
        //        data.submit();
    },

    _onSubmitUpload: function (e, data) {
        data.formData = { Key: this.currentUploadKey, OriginalFileName: data.files[0].name };
    },

    _onUploadDone: function (e, data) {
        var self = this;
        this.group.set('Avatar', data.result);
        this.currentUploadKey++;
        var mediaResource = new Bowerbird.Models.MediaResource({ Key: self.CurrentUploadKey });
        this.avatarItemView = new Bowerbird.Views.AvatarItemView({ mediaResource: mediaResource });
        $('#avatar-add-pane').hide();
        $('#avatar-viewer').append(this.avatarItemView.render().el);
        loadImage(
            data.files[0],
            function (img) {
                if (img instanceof HTMLImageElement) { // FF seems to fire this handler twice, on second time returning error, which we ignore :(
                    self.avatarItemView.showTempMedia(img);
                    $('#media-resource-avatar').animate({ scrollLeft: 100000 });
                }
            },
            {
                maxHeight: 220
            }
        );
    },

    removeMediaResource: function () {
        this.group.set('Avatar', null);
        this.avatarItemView = null;
        ich.AvatarChooseFile().appendTo(this.$el.find('#avatar-add-pane'));
        this._initMediaUploader();
    }

});