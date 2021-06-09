<template>
    <div class="mb-5">
        <div class="text-center">
            <i class="fa fa-pencil-square-o fa-2x" aria-hidden="true"></i> <h3 style="position: relative">New Article</h3>
        </div>
        <div class="text-right">
            <button class="btn btn-primary" @click="changeComponent">
                <i class="material-icons" style="vertical-align: middle;">
                    arrow_back
                </i>Return to Articles
            </button>
        </div>
        <div v-if="loading" class="text-center">

            <pulse-loader color="#31A3DD"></pulse-loader>
        </div>
        <div v-else>

            <!-- Formulario del articulo -->

            <div class="container">
                <div class="col-sm-12">
                    <div class="row">
                        <div class="col-sm-3"></div>
                        <div class="col-sm-6 card m-2">
                            <div class="card-body text-center">
                                <div class="card-title">
                                    <h5>Orientation</h5>
                                </div>
                                <b-form-group>
                                    <b-form-checkbox-group v-model="selectedori" :options="orientations">
                                    </b-form-checkbox-group>
                                </b-form-group>
                                <span style="color: red"><strong>{{errororientation}}</strong></span>
                            </div>
                        </div>
                        <div class="col-sm-3"></div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 mt-2 card  ">
                            <div class="card-title mt-2">
                                <h5>General Info</h5>
                            </div>
                            <div class="row m-3 card-body">
                                <div class="col-sm-8 col-md-8 col-lg-8 mb-2">
                                    <span>Title</span>
                                    <input id="title" @focus="maxLenght" class="form-control" type="text" v-model="title" @blur="buildUrl">
                                </div>
                                <div class="col-sm-4 col-md-4 col-lg-4 mb-2">
                                    <span>Status</span>
                                    <v-select v-model="status" :options="activeinactive"></v-select>
                                </div>
                                <div class="col-sm-12 mb-2">
                                    <span>Short Content</span>
                                    <textarea class="form-control" type="text" v-model="shortcontent"></textarea>
                                </div>
                                <div class="col-sm-4 col-md-4 col-lg-4 mb-2">
                                    <span>Structure</span>
                                    <v-select v-model="structure" :options="structures"></v-select>
                                </div>
                                <div class="col-sm-4 col-md-4 col-lg-4 mb-2">
                                    <span>Type of Article</span>
                                    <v-select v-model="pagetype" :options="pagestypes"></v-select>
                                </div>
                                <div class="col-sm-4 col-md-4 col-lg-4 mb-2">
                                    <span>Author</span>
                                    <v-select v-model="copywriter" :options="copywriters"></v-select>
                                </div>
                                <div class="col-sm-4 col-md-4 col-lg-4 mb-2">
                                    <span>Publication Date</span>
                                    <div class="form-group">
                                        <div class="input-group date" id="datetimepicker" data-target-input="nearest">
                                            <input type="text" prefix="calendar" v-on:blur="calendarInput" id="calendario" class="form-control datetimepicker-input" data-target="#datetimepicker" :value="today" />
                                            <div class="input-group-append" data-target="#datetimepicker" data-toggle="datetimepicker">
                                                <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4 col-md-4 col-lg-4 mb-2 mt-4">
                                    <b-form-checkbox id="checkbox1"
                                                     v-model="showinmenu">
                                        Show in Home Page
                                    </b-form-checkbox>
                                </div>
                                <div class="col-sm-12">
                                    <span style="color: red"><strong>{{error}}</strong></span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="card col-sm-12">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-sm-6 col-md-6 col-lg-6 mt-4">
                                        <div>
                                            <h5>Title Seo</h5>
                                        </div>
                                        <div class="input-group">
                                            <input type="text" class="form-control" id="inlineFormInputGroup" v-model="titleseo">
                                        </div>
                                    </div>
                                    <div class="col-sm-6 col-md-6 col-lg-6 mt-4">
                                        <div>
                                            <h5>Friendly URL</h5>
                                        </div>
                                        <div class="input-group">
                                            <div class="input-group">
                                                <input type="text" class="form-control" id="inlineFormInputGroup" v-model="friendlyurl">
                                            </div>

                                        </div>
                                    </div>
                                    <div class="col-sm-12">
                                        <span style="color: red"><strong>{{errorseo}}</strong></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>



                    <div class="row">
                        <div class="col-sm-12 mt-2 card  ">
                            <div class="row mb-2 card-body">
                                <div class=" col-sm-12">
                                    <div class="">
                                        <div class="">
                                            <h5>Title With Formart</h5>
                                        </div>
                                        <div class="mb-3">
                                            <ckeditor :editor="editor"  @ready="onReady" v-model="titleformat"></ckeditor>

                                        </div>
                                        <div class="">
                                            <h5>Content</h5>
                                        </div>
                                        <div>
                                            <ckeditor :editor="editor"  @ready="onReady" v-model="content"></ckeditor>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 card mt-2">
                            <div class="card-body ">
                                <div class="card-title">
                                    <h5>Images</h5>
                                </div>
                                <div class="col-sm-6 col-md-4 col-lg-4">

                                    <div class="custom-file">
                                        <input hidden type="file" ref="files" class="custom-file-input" id="customFileLang" multiple @change="upload($event)">
                                        <label class="custom-file-label" for="customFileLang">Choose File</label>
                                    </div>
                                </div>

                                <div id="preview" class="col-sm-12">
                                    <div class="row">

                                        <div class="col-sm-3" v-for="(url, key) in urls" v-if="url">

                                            <b-card :img-src="url"
                                                    img-alt="Image"
                                                    img-top
                                                    tag="article"
                                                    style="max-width: 30rem;"
                                                    class="mb-2">

                                                <b-button @click="removePicture(key)" variant="danger"> <i class="material-icons md-48" style="vertical-align: middle;"> delete</i></b-button>
                                            </b-card>

                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row mt-2">
                        <div class="col-sm-12">
                            <div class="text-right">
                                <button class="btn btn-primary" @click="saveArticle">Save</button>
                            </div>
                        </div>
                    </div>



                </div>
            </div>

            <!-- ********************* -->
        </div>
    </div>




</template>

<script>

    import { bus } from './app.js';
    import DecoupledEditor from '@ckeditor/ckeditor5-build-decoupled-document';
    import CKEditor from '@ckeditor/ckeditor5-vue';
    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
    Vue.component('v-select', vSelect)
    var arreglo = [];
    export default {
        props: ['cards'],
        data: function () {
            return {
                error: '',
                pagestypes:[],
                titleseo: '',
                pagetype: '',
                today: '',
                publicationdate: '',
                copywriters: [],
                copywriter:[],
                staticurl: '/articles/',
                friendlyurl:'',
                title: '',
                loading: true,
                editor: DecoupledEditor,
                perPage: 5,
                currentPage: 1,
                countrows: 2,
                url: '',
                showinmenu: false,
                // campos del hotel
                hotelname: '',
                shorcontent:'',
                urls: [],
                errororientation: '',
                files: [],
                type: 1,
                errorseo:'',
                orientations: [],
                orientation: '',
                selectedori: [],
                content: '',
                images: [],
                typerequest: '',
                formdata: [],
                tempevent: '',
                status: null,
                structures: [],
                structure: [],
                shortcontent: '',
                titleformat:'',
                slider: {
                    value: false,
                    label: "No Show"
                },
                sliders: [
                    {
                        value: true,
                        label: "Show"
                    },
                    {
                        value: false,
                        label: "No Show"
                    }
                ],
                activeinactive: [
                    {
                        value: 1,
                        label: "Active"
                    },
                    {
                        value: 0,
                        label: "Inactive"
                    }
                ],
               
                //*************


            }
        },
        components: {

            'PulseLoader': PulseLoader,
            ckeditor: CKEditor.component,



        },
        methods: {
            maxLenght: function () {
                document.getElementById("title").maxLength = "90";
            },
            calendarInput: function () {

                this.publicationdate = $('#calendario').val();
                var date = new Date(this.publicationdate.substr(0, 10));
                console.log(date);
                var dia = date.getUTCDate();
                var mes = date.getUTCMonth();
                var anio = date.getUTCFullYear();
                var fecha = new Date(anio, mes, dia);
                var month = fecha.getUTCMonth() + 1;
                var day = fecha.getUTCDate();
                this.today = day.toString().padStart(2, "0") + "-" + month.toString().padStart(2, "0") + "-" + fecha.getUTCFullYear();
                this.$forceUpdate();

            },
            onReady(editor) {
                // Insert the toolbar before the editable area.
                editor.ui.view.editable.element.parentElement.insertBefore(
                    editor.ui.view.toolbar.element,
                    editor.ui.view.editable.element
                );
            },
            buildUrl: function (evento) {
                if (this.titleseo == "") {
                    
                    var url = this.title.toLowerCase().split(' ').join('-');
                    url = url.toLowerCase();
                    url = url.replace(/---/g, '-');
                    url = url.replace(/--/g, '-');
                    url = url.replace(/ /g, '-');
                    url = url.replace(/\?/g, '');
                    url = url.replace(/!/g, '');
                    url = url.replace(/,/g, '');
                    url = url.replace(/á/g, 'a');
                    url = url.replace(/é/g, 'e');
                    url = url.replace(/í/g, 'i');
                    url = url.replace(/ó/g, 'o');
                    url = url.replace(/ú/g, 'u');
                    url = url.replace(/'/g, '');
                    url = url.replace(/"/g, '');
                    url = url.replace(/’/g, '');
                    url = url.replace(/”/g, '');
                    url = url.replace(/“/g, '');
                    url = url.replace(/&/g, 'and');
                    url = url.replace(/ñ/g, 'n');
                    url = url.replace(/=/g, '');
                    var urls = "/magazine/" + url;
                    this.titleseo = this.title;
                    this.friendlyurl = urls;
                }
               
            },
            getData: function () {

                axios.get('/Content/management/getDataPlaces', {


                }).then(response => {
                    this.orientations = response.data[2][0];
                    this.structures = response.data[7][0];
                    this.copywriters = response.data[8][0];
                    this.copywriter = this.copywriters[0];
                    this.pagestypes = response.data[9][0]
                    this.loading = false;

                });
            },
            removePicture: function (key) {
                this.urls.splice(key, 1);
                this.files.splice(key, 1);
                this.tempevent = this.files;
                console.log("event lenght borrado " + this.tempevent.length);
            },
            upload: function (event) {

                // this.url = URL.createObjectURL(file);
                let uploadedFiles = this.$refs.files.files;

                /*
                  Adds the uploaded file to the files array
                */
                for (var i = 0; i < uploadedFiles.length; i++) {
                    this.files.push(uploadedFiles[i]);
                }
                for (var i = 0; i < event.target.files.length; i++) {

                    this.urls.push(URL.createObjectURL(event.target.files[i]));
                }
                this.tempevent = event.target.files;

            },
            changeComponent: function () {
                if (this.title !== "" || this.content !== "") {
                    var r = this;
                    $.confirm({
                        title: 'Changes without save',
                        content: 'You have changes without save, Do you want to continue?',
                        typeAnimated: true,
                        type: 'red',
                        buttons: {
                            delete: {
                                text: 'Continue',
                                btnClass: 'btn-blue',
                                action: function () {
                                    r.$root.currentarticles = "articles";
                                }
                            },
                            Cancel: function () {
                            },
                        }
                    });
                } else {
                    this.$root.currentarticles = "articles";
                }

            },
            getFields: function () {
                console.log("scrtuctore value: "+this.structure.value);
                var date = new Date();
                console.log(date);
                var article = {
                    title: this.title,
                    placetypeid: 1,
                    status: this.status.value,
                    content: this.content,
                    orientations: this.selectedori,
                    structure: this.structure.value,
                    date: date,
                    sysitem: 9,
                    staticurl: this.staticurl,
                    titleseo: this.titleseo,
                    friendlyurl: this.friendlyurl,
                    type: this.pagetype.value,
                    shortcontent: this.shortcontent,
                    titleformat: this.titleformat,
                    author: this.copywriter.value,
                    publicationdate: this.publicationdate,
                    showinmenu: this.showinmenu,


                };
                return article;
            },
            verifyFields: function () {
                console.log("content : " + this.content);
                this.errororientation = "";
                if (this.title.trim() == "" || this.status == "" || this.publicationdate == ""
                 || this.status == null || this.structure == "" || this.structure == null || this.copywriter == null) {
                    this.error = "Please complete all the require fileds.";

                    return true;
                } else {
                    this.error = "";
                    if (this.selectedori.length < 1) {
                        if (this.selectedori.length < 1) {
                            this.errororientation = "Please, select at least one orientation."
                            return true;
                        } else {
                            this.errororientation = ""
                        } 
                        return true;
                    } else {
                        this.errororientation = ""
                        if (this.titleseo == "" || this.friendlyurl == "") {
                            this.errorseo = "Please complete all the require fileds.";
                            return true;
                        } else {
                            this.errorseo = "";
                        }


                    }
                    return false;
                }
            },
            saveArticle: function () {

                console.log("antes del verify");
                var error = this.verifyFields();
                console.log("despues del verify");
                if (!error) {
                    var article = this.getFields();
                } else {
                    return this;
                }

                console.log("se va para save");
                var text = "Do you want to add " + article.title + " article?";

                let files = new FormData();
                // let file = event.target.files[0];
                console.log("event lenght " + this.files.length);
                for (var i = 0; i < this.files.length; i++) {
                    files.append('file', this.files[i])

                }
                this.formdata = files;

                var data = JSON.stringify(article);
                console.log("el json es " +data);
                var r = this;
                $.confirm({
                    title: 'New Article ',
                    content: text,
                    typeAnimated: true,
                    type: 'blue',
                    buttons: {
                        save: {
                            text: 'Continue',
                            btnClass: 'btn-blue',
                            action: function () {
                                r.loading = true;
                                r.typerequest = "Please wait a moment...";
                                console.log("article: " + article);

                                axios.post('/Content/management/addArticle', {
                                    data: data,

                                }).then(response => {
                                    console.log("response del addArticle: "+ response.data);
                                    if (response.data !== "ok") {
                                        var id = response.data;
                                        let config = {
                                            header: {
                                                'Content-Type': 'multipart/form-data'
                                            }
                                        }

                                        if (r.files.length > 0) {
                                            files.append('id', id);
                                            files.append('sys', 9)
                                            axios.post('/Content/management/uploadImage', r.formdata, config).then(
                                                response => {
                                                    if (response.data == "ok") {
                                                        r.files = [];
                                                        r.files = [];
                                                        r.tempevent = [];
                                                        r.typerequest = "";
                                                        r.urls = [];
                                                        r.$refs.files.files.value = "";
                                                    }
                                                }
                                            );
                                        }

                                        r.loading = false;
                                        $.alert('Article added succefully');
                                        r.cleanFields();
                                       
                                    }
                                    
                                });
                            }
                        },
                        Cancel: function () {
                        },
                    }
                });
            },

            cleanFields: function () {
                this.title = "";
                this.selectedori = [];
                this.status = "";
                this.structure = "";
                this.content = "";
                this.titleformat = "";
                this.shortcontent = "";
                this.titleseo = "";
                this.friendlyurl = "";





            },
            
        },
        mounted() {
            
            this.getData();
            $(function () {
                $('#datetimepicker').datetimepicker({
                    format: 'DD-MM-YYYY'
                });
            });
            bus.$on('maps', obj => {
                console.log(obj);
                this.longitude = obj.lon;
                this.latitude = obj.lat;
            });
        },

    }
    

</script>
