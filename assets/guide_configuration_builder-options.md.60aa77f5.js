import{_ as t,c as e,o as i,a as o}from"./app.9dbf2506.js";const m=JSON.parse('{"title":"Builder options","description":"","frontmatter":{},"headers":[{"level":2,"title":"DateFormat","slug":"dateformat","link":"#dateformat","children":[]},{"level":2,"title":"ForField","slug":"forfield","link":"#forfield","children":[]}],"relativePath":"guide/configuration/builder-options.md"}'),a={name:"guide/configuration/builder-options.md"},l=o('<h1 id="builder-options" tabindex="-1">Builder options <a class="header-anchor" href="#builder-options" aria-hidden="true">#</a></h1><h2 id="dateformat" tabindex="-1">DateFormat <a class="header-anchor" href="#dateformat" aria-hidden="true">#</a></h2><p>A property to get/set date format used for value matching when filtering on properties of <code>DateTime</code> type.</p><ul><li>Type: <code>string</code></li><li>Default: <code>CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern</code> (MM/dd/yyyy)</li></ul><h2 id="forfield" tabindex="-1">ForField <a class="header-anchor" href="#forfield" aria-hidden="true">#</a></h2><p>A method to customize <a href="./field-options.html">individual field options</a>.</p><p>Arguments:</p><table><thead><tr><th style="text-align:left;">Name</th><th style="text-align:left;">Type</th><th style="text-align:left;">Comment</th></tr></thead><tbody><tr><td style="text-align:left;">property</td><td style="text-align:left;"><code>Expression&lt;Func&lt;TDataTableFields, TMember&gt;&gt;</code></td><td style="text-align:left;">Expression to the field</td></tr><tr><td style="text-align:left;">optionsAction</td><td style="text-align:left;"><code>Action&lt;FieldOptions&lt;TEntity&gt;&gt;</code></td><td style="text-align:left;">An action to configure the field options</td></tr></tbody></table>',8),d=[l];function r(n,s,c,f,h,p){return i(),e("div",null,d)}const _=t(a,[["render",r]]);export{m as __pageData,_ as default};
