const path = require('path');
const VueLoaderPlugin = require('vue-loader/lib/plugin');

module.exports = {
    entry: {
        //membership: './ViewModels/Components/Membership/app.js',
        //content: './ViewModels/Components/Content/app.js',
        workflows: './ViewModels/Components/Workflows/app.js'

},
    output: {
        path: path.resolve(__dirname, './ViewModels/Components'),
        filename: '[name].js'

    },
    // IMPORTANT NOTE: If you are using Webpack 2 or above, replace "loaders" with "rules"
    module: {
        rules: [
            {
                test: /\.vue$/,
                loader: 'vue-loader'
            },
            // this will apply to both plain `.js` files
            // AND `<script>` blocks in `.vue` files
           
            // this will apply to both plain `.css` files
            // AND `<style>` blocks in `.vue` files
            {
                test: /\.css$/,
                use: [
                    'vue-style-loader',
                    'css-loader'
                ]
            }
        ]
    },
    plugins: [
        // make sure to include the plugin for the magic
        new VueLoaderPlugin(),
    
        
    ],
    resolve: {
        alias: {
            vue: 'vue/dist/vue.js',
        }
    },
    watchOptions: {
        ignored: /node_modules/,
        poll: true,
    },
}