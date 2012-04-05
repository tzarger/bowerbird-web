﻿
window.Bowerbird.Views.AvatarItemView = Backbone.View.extend({
    className: 'avatar-uploaded',

    events: {
        'click .view-media-resource-button': 'viewMediaResource',
        'click .add-caption-button': 'viewMediaResource',
        'click .remove-media-resource-button': 'removeMediaResource'
    },

    //template: $.template('avatarMediaResourceUploadedTemplate', $('#avatar-media-resource-uploaded-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'showTempMedia', 'showUploadedMedia');
        this.mediaResource = options.mediaResource;
        this.mediaResource.on('change:mediumImageUri', this.showUploadedMedia);
    },

    render: function () {
        //$.tmpl('avatarMediaResourceUploadedTemplate', this.mediaResource.toJSON()).appendTo(this.$el);
        var avatarUploaded = ich.avataruploaded(this.mediaResource.toJSON()).appendTo(this.$el);
        return this;
    },

    viewMediaResource: function () {
        alert('Coming soon');
    },

    removeMediaResource: function () {
        this.remove();
        //        this.trigger('removeMediaResource', this);
        //var avatarChooseFile = ich.avatarchoosefile().appendTo($('#media-uploader'));
        $('#avatar-add-pane').show();
    },

    showTempMedia: function (img) {
        this.$el.find('div:first-child img').replaceWith($(img));
    },

    showUploadedMedia: function (mediaResource) {
        this.$el.find('div:first-child img').replaceWith($('<img src="' + mediaResource.get('mediumImageUri') + '" alt="" />'));
    }
});