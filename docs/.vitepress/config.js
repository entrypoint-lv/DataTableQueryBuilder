export default {
    title: 'DataTable Query Builder',
    description: 'Server-side .NET query builder for JavaScript datatables',
    base: '/DataTableQueryBuilder/',

    themeConfig: {
        nav: [
            { text: 'Guide', link: '/guide/' },
        ],

        socialLinks: [
            { icon: 'github', link: 'https://github.com/EntryPointDev/DataTableQueryBuilder' },
        ],

        sidebar: [
            {
                text: 'Introduction',
                items: [
                    { text: 'Getting Started', link: '/guide/' },
                    { text: 'Basic Usage', link: '/guide/basic-usage.md' },
                    { text: 'How It Works', link: '/guide/how-it-works.md' },
                    { text: 'Value Matching', link: '/guide/value-matching.md' },
                    
                ]
            },
            {
                text: 'Advanced',
                items: [
                    { text: 'Custom Expressions', link: '/guide/advanced/custom-search-sort.md' },
                    { text: 'Usage without Projection', link: '/guide/advanced/usage-without-projection.md' },
                ]
            },
            {
                text: 'Configuration',
                items: [
                    { text: 'Builder Options', link: '/guide/configuration/builder-options.md' },
                    { text: 'Field Options', link: '/guide/configuration/field-options.md' },
                ]
            },
            {
                text: 'Examples',
                items: [
                    { text: 'Demo and Samples', link: '/guide/samples.md' },
                ]
            }
        ],

        footer: {
            message: 'Released under the MIT License.',
            copyright: 'Copyright © 2019 - present <a href="http://entrypoint.lv">Entrypoint</a>'
        }
    }
}