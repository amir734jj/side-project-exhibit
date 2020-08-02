const gulp = require('gulp');
const browserify = require('browserify');
const babelify = require('babelify');
const source = require('vinyl-source-stream');
const uglify = require('gulp-uglify');
const buffer      = require('vinyl-buffer');

babelify.configure({
    babelrc: './.babelrc'
});

gulp.task('build', function () {
    return browserify({entries: 'Api/wwwroot/scripts/script.js', extensions: ['.js'], debug: false})
        .transform(babelify)
        .bundle()
        .pipe(source('script.js'))
        .pipe(buffer())
        .pipe(uglify())
        .pipe(gulp.dest('client-build'));
});
