﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using GraphQLParser.Native;

namespace NativeConsoleTest
{
    class Program
    {
        private static readonly string KitchenSink = @"
query queryName($foo: ComplexType, $site: Site = MOBILE) {
  whoever123is: node(id: [123, 456]) {
    id ,
    ... on User @defer {
      field2 {
        id ,
        alias: field1(first:10, after:$foo,) @include(if: $foo) {
          id,
          ...frag
        }
      }
    }
    ... @skip(unless: $foo) {
      id
    }
    ... {
      id
    }
  }
}

mutation likeStory {
  like(story: 123) @defer {
    story {
      id
    }
  }
}

subscription StoryLikeSubscription($input: StoryLikeSubscribeInput) {
  storyLikeSubscribe(input: $input) {
    story {
      likers {
        count
      }
      likeSentence {
        text
      }
    }
  }
}

fragment frag on Friend {
  foo(size: $size, bar: $b, obj: {key: ""value""})
}

{
  unnamed(truthy: true, falsey: false),
  query
}";
        static void Main(string[] args)
        {
            var document = GraphQlParser.ParseString(KitchenSink);
          Debug.Assert(document.IsSuccess);

          document.Dispose();
            Console.WriteLine("Hello World!");
        }
    }
}