﻿
window.Bowerbird.Models.GroupChat = Bowerbird.Models.Chat.extend({
    defaults: {
        group: null
    },
    initialize: function (options) {
        this.constructor.__super__.initialize.apply(this, [options])
        _.extend(this, Backbone.Events);
        _.bindAll(this);
        this.set('group', options.group);
    }
});