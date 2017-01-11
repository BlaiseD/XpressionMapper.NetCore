import { ContosoUniversityPage } from './app.po';

describe('contoso-university App', function() {
  let page: ContosoUniversityPage;

  beforeEach(() => {
    page = new ContosoUniversityPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
