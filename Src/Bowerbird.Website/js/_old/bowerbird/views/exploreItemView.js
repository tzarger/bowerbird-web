﻿
window.Bowerbird.Views.ExploreItemView = Backbone.View.extend({
    className: 'explore-item',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.exploreItem = options.group;
    },

    render: function () {
        var json = this.exploreItem.toJSON();
        var html = '';
        var className = '';
        switch (this.exploreItem.get('Type')) {
            case 'organisation':
                html = ich.OrganisationItem(json);
                className = 'organisation-explore-item';
                break;
            case 'team':
                html = ich.TeamItem(json);
                className = 'team-explore-item';
                break;
            case 'project':
                html = ich.ProjectItem(json);
                className = 'project-explore-item';
                break;
            default:
                break;
        }
        this.$el.append(html).addClass(className);
        return this;
    }
});