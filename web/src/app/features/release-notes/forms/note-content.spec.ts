import { toContentRequest } from './note-content';

describe('toContentRequest', () => {
  it('normalises HTML, drops empty sections and orders sections for display', () => {
    expect(
      toContentRequest({
        summaryHtml: '<p>Hello&nbsp;world</p>',
        sections: [
          { category: 'fixed', contentHtml: '<p>Crash</p>' },
          { category: 'security', contentHtml: '<p><br></p>' },
          { category: 'breaking', contentHtml: '<p>v1&nbsp;removed</p>' },
        ],
      }),
    ).toEqual({
      summaryHtml: '<p>Hello world</p>',
      sections: [
        { category: 'breaking', contentHtml: '<p>v1 removed</p>' },
        { category: 'fixed', contentHtml: '<p>Crash</p>' },
      ],
    });
  });

  it('treats an empty editor as no summary', () => {
    expect(toContentRequest({ summaryHtml: '<p><br></p>', sections: [] })).toEqual({
      summaryHtml: '',
      sections: [],
    });
  });
});
