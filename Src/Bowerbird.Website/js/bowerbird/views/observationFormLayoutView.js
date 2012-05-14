﻿/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationFormLayoutView
// -------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/editmapview', 'views/editmediaview', 'datepicker', 'multiselect'], function ($, _, Backbone, app, ich, EditMapView, EditMediaView) {

    var ObservationFormLayoutView = Backbone.Marionette.Layout.extend({
        tagName: 'section',

        className: 'form single-medium',

        id: 'observation-form',

        regions: {
            media: '#media-resources-fieldset',
            map: '#location-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Title': '_contentChanged',
            'change input#ObservedOn': '_contentChanged',
            'change input#Address': '_contentChanged',
            'change input#Latitude': '_latLongChanged',
            'change input#Longitude': '_latLongChanged',
            'change input#AnonymiseLocation': '_anonymiseLocationChanged',
            'change #projects-field input:checkbox': '_projectsChanged',
            'change #category-field input:checkbox': '_categoryChanged',
            'click #media-resource-import-button': '_showImportMedia'
        },

        initialize: function (options) {
            //            _.bindAll(this,
            //            'render'
            //            'start',
            //            '_showImportMedia',
            //            '_contentChanged',
            //            '_latLongChanged',
            //            '_anonymiseLocationChanged',
            //            '_projectsChanged',
            //            '_categoryChanged',
            //            '_cancel',
            //            '_save'
            //            );
            //this.observation = options.observation;
            //this.editMediaView = new Bowerbird.Views.EditMediaView({ el: $('#media-resources-fieldset'), observation: this.model });
            //this.editMapView = new Bowerbird.Views.EditMapView({ observation: this.model });
        },

        onRender: function () {
            var editMapView = new EditMapView({ el: '#location-fieldset', model: this.model });
            this.map.attachView(editMapView);
            editMapView.render();

            var editMediaView = new EditMediaView({ el: '#media-resources-fieldset', model: this.model });
            this.media.attachView(editMediaView);
            editMediaView.render();

            this.observedOnDatePicker = $('#ObservedOn').datepicker();

            this.categoryListSelectView = $("#Category").multiSelect({
                selectAll: false,
                singleSelect: true,
                noneSelected: '<span>Select Category</span>',
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<span />');
                    _.each(selectedOptions, function (option) {
                        $selectedHtml.append('<span>' + option.text + '</span> ');
                    });
                    return $selectedHtml.children();
                }
            });

            ich.addTemplate('ProjectSelectItem', '{{#Projects}}<option value="{{Id}}">{{Name}}</option>{{/Projects}}');

            // Add project options
            this.$el.find('#Projects').append(ich.ProjectSelectItem({ Projects: app.authenticatedUser.projects.toJSON() }));

            this.projectListSelectView = this.$el.find("#Projects").multiSelect({
                selectAll: false,
                messageText: 'You can select more than one project',
                noneSelected: '<span>Select Projects</span>',
                renderOption: function (id, option) {
                    var html = '<label><input style="display:none;" type="checkbox" name="' + id + '[]" value="' + option.value + '"';
                    if (option.selected) {
                        html += ' checked="checked"';
                    }
                    var project = app.authenticatedUser.projects.get(option.value);

                    html += ' /><img src="' + project.get('Avatar').UrlToImage + '" alt="" />' + project.get('Name') + '</label>';
                    return html;
                },
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<div />');
                    _.each(selectedOptions, function (option) {
                        var project = app.authenticatedUser.projects.get(option.value);
                        $selectedHtml.append('<span class="selected-project"><img src="' + project.get('Avatar').UrlToImage + '" alt="" />' + option.text + '</span> ');
                    });
                    return $selectedHtml.children();
                }
            });

            //var myScroll = new iScroll('media-uploader', { hScroll: true, vScroll: false });
        },

        _showImportMedia: function () {
            alert('Coming soon');
        },

        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);

            if (target.attr('id') === 'Address') {
                this._latLongChanged(e);
            }
        },

        _latLongChanged: function (e) {
            var oldPosition = { latitude: this.model.get('Latitude'), longitude: this.model.get('Longitude') };
            var newPosition = { latitude: $('#Latitude').val(), longitude: $('#Longitude').val() };

            this.model.set('Latitude', newPosition.latitude);
            this.model.set('Longitude', newPosition.longitude);

            // Only update pin if the location is different to avoid infinite loop
            if (newPosition.Latitude != null && newPosition.Longitude != null && (oldPosition.Latitude !== newPosition.Latitude || oldPosition.Longitude !== newPosition.Longitude)) {
                this.editMapView.changeMarkerPosition(this.model.get('Latitude'), this.model.get('Longitude'));
            }
        },

        _anonymiseLocationChanged: function (e) {
            var $checkbox = $(e.currentTarget);
            this.model.set({ AnonymiseLocation: $checkbox.attr('checked') == 'checked' ? true : false });
        },

        _projectsChanged: function (e) {
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                var projectId = $checkbox.attr('value');
                this.model.addProject(projectId);
            } else {
                this.model.removeProject($checkbox.attr('value'));
            }
        },

        _categoryChanged: function (e) {
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                this.model.set('Category', $checkbox.attr('value'));
            } else {
                this.model.set('Category', '');
            }
        },

        _cancel: function () {
            //            app.set('newObservation', null);
            //            app.appUserGroupRouter.navigate(app.stream.get('Uri'), { trigger: false });
            //            this.trigger('formClosed', this);
        },

        _save: function () {
            this.model.save();
            //            app.appRouter.navigate(app.stream.get('Uri'), { trigger: false });
            //            this.trigger('formClosed', this);
        }
    });

    return ObservationFormLayoutView;

});